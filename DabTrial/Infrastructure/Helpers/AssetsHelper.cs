using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Web.Mvc;
using System.Web;
using System;

namespace MvcHtmlHelpers
{
    //http://stackoverflow.com/questions/5110028/add-css-or-js-files-to-layout-head-from-views-or-partial-views#5148224
    public static partial class HtmlExtensions
    {
        public static AssetsHelper Assets(this HtmlHelper htmlHelper)
        {
            return AssetsHelper.GetInstance(htmlHelper);
        }
    }
    [Flags]
    public enum BrowserType { Ie6=0x1,Ie7=0x2,Ie8=0x4,IeLegacy=0x7,W3cCompliant=0x8,All=0x17 }
    public class AssetsHelper
    {
        public static AssetsHelper GetInstance(HtmlHelper htmlHelper)
        {
            var instanceKey = "AssetsHelperInstance";
            var context = htmlHelper.ViewContext.HttpContext;
            if (context == null) {return null;}
            var assetsHelper = (AssetsHelper)context.Items[instanceKey];
            if (assetsHelper == null){context.Items.Add(instanceKey, assetsHelper = new AssetsHelper(htmlHelper));}
            return assetsHelper;
        }
        private readonly List<string> _styleRefs = new List<string>();
        public AssetsHelper AddStyle(string stylesheet)
        {
            _styleRefs.Add(stylesheet);
            return this;
        }
        private readonly List<string> _scriptRefs = new List<string>();
        private readonly List<string> _headerScriptRefs = new List<string>();
        public AssetsHelper AddScript(string scriptfile)
        {
            _scriptRefs.Add(scriptfile);
            return this;
        }
        public AssetsHelper AddHeaderScript(string scriptfile)
        {
            _headerScriptRefs.Add(scriptfile);
            return this;
        }
        public IHtmlString RenderStyles()
        {
            ItemRegistrar styles = new ItemRegistrar(ItemRegistrarFormatters.StyleFormat,_urlHelper);
            styles.Add(Libraries.UsedStyles());
            styles.Add(_styleRefs);
            return styles.Render();
        }
        public IHtmlString RenderHeaderScripts()
        {
            if (!_headerScriptRefs.Any()) { return new HtmlString(String.Empty); }
            ItemRegistrar scripts = new ItemRegistrar(ItemRegistrarFormatters.ScriptFormat, _urlHelper);
            scripts.Add(_headerScriptRefs);
            return scripts.Render();
        }
        public IHtmlString RenderScripts()
        {
            ItemRegistrar scripts = new ItemRegistrar(ItemRegistrarFormatters.ScriptFormat, _urlHelper);
            scripts.Add(Libraries.UsedScripts());
            scripts.Add(_scriptRefs);
            return scripts.Render();
        }
        public LibraryRegistrar Libraries { get; private set; }
        private UrlHelper _urlHelper;
        public AssetsHelper(HtmlHelper htmlHelper)
        {
            _urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
            Libraries = new LibraryRegistrar();
        }
    }
    public class LibraryRegistrar
    {
        public class Component
        {
            internal class HtmlReference
            {
                internal string Url { get; set; }
                internal BrowserType ServeTo { get; set; }
            }
            internal List<HtmlReference> Styles { get; private set; }
            internal List<HtmlReference> Scripts { get; private set; }
            internal List<string> PreceedingLibraries { get; private set; }
            internal bool IsImplementingLibrary { get; set; }

            public Component()
            {
                Styles = new List<HtmlReference>();
                Scripts = new List<HtmlReference>();
                PreceedingLibraries = new List<string>();
            }
            public Component Requires(params string[] libraryNames)
            {
                foreach (var lib in libraryNames)
                {
                    if (!PreceedingLibraries.Contains(lib))
                    {
                        PreceedingLibraries.Add(lib);
                    }
                }
                return this;
            }
            public Component AddStyle(string url, BrowserType serveTo = BrowserType.All)
            {
                Styles.Add(new HtmlReference { Url = url, ServeTo=serveTo });
                return this;
            }
            public Component AddScript(string url, BrowserType serveTo = BrowserType.All)
            {
                Scripts.Add(new HtmlReference { Url = url, ServeTo = serveTo });
                return this;
            }
        }
        private readonly Dictionary<string, Component> _allLibraries = new Dictionary<string, Component>();
        private List<string> _usedLibraries = new List<string>();
        internal IEnumerable<string> UsedScripts()
        {
            SetOrder();
            var returnVal = new List<string>();
            foreach (var key in _usedLibraries)
            {
                returnVal.AddRange(from s in _allLibraries[key].Scripts
                                   where IncludesCurrentBrowser(s.ServeTo)
                                   select s.Url);
            }
            return returnVal;
        }
        internal IEnumerable<string> UsedStyles()
        {
            SetOrder();
            var returnVal = new List<string>();
            foreach (var key in _usedLibraries)
            {
                returnVal.AddRange(from s in _allLibraries[key].Styles
                                   where IncludesCurrentBrowser(s.ServeTo)
                                   select s.Url);
            }
            return returnVal;
        }
        public void Uses(params string[] libraryNames)
        {
            foreach (var name in libraryNames)
            {
                if (!_usedLibraries.Contains(name)){_usedLibraries.Add(name);}
            }
        }
        public bool IsUsing(string libraryName)
        {
            SetOrder();
            return _usedLibraries.Contains(libraryName);
        }
        private List<string> WalkLibraryTree(IEnumerable<string> libraryNames)
        {
            var orderedLibs = new List<string>(libraryNames);
            if (orderedLibs.Count == 0) { return orderedLibs; }
            int counter = 0;
            foreach (string libraryName in libraryNames)
            {
                WalkLibraryTree(libraryName, ref orderedLibs, ref counter);
            }
            var implementingLibs = new List<string>();
            var returnList = new List<string>(orderedLibs.Count);
            for (int i = orderedLibs.Count-1; i >= 0; --i)
            {
                string currentLib = orderedLibs[i];
                if (_allLibraries[currentLib].IsImplementingLibrary)
                {
                    implementingLibs.Add(currentLib);
                }
                else
                {
                    returnList.Add(currentLib);
                }
            }
            returnList.AddRange(implementingLibs);
            return returnList;
        }
        private void WalkLibraryTree(string libraryName, ref List<string> libBuild, ref int counter)
        {
            if (counter++ > 1000) { throw new System.Exception("Dependancy library appears to be in infinate loop - please check for circular reference"); }
            Component library;
            if (!_allLibraries.TryGetValue(libraryName, out library))
                { throw new KeyNotFoundException("Cannot find a definition for the required style/script library named: " + libraryName); }
            foreach (var childLibraryName in library.PreceedingLibraries)
            {
                int childIndex = libBuild.IndexOf(childLibraryName);
                if (childIndex!=-1)
                {
                    //child already exists, so move parent to position before child if it isn't before already
                    int parentIndex = libBuild.IndexOf(libraryName);
                    if (parentIndex>childIndex)
                    {
                        var parentLib = libBuild[parentIndex];
                        libBuild.RemoveAt(parentIndex);
                        libBuild.Insert(childIndex,parentLib);
                    }
                }
                else
                {
                    libBuild.Add(childLibraryName);
                    WalkLibraryTree(childLibraryName, ref libBuild, ref counter);
                }
            }
            return;
        }
        private bool _dependenciesExpanded;
        private void SetOrder()
        {
            if (_dependenciesExpanded){return;}
            _usedLibraries = WalkLibraryTree(_usedLibraries);
            _dependenciesExpanded = true;
        }
        public Component this[string index]
        {
            get
            {
                if (_allLibraries.ContainsKey(index))
                    { return _allLibraries[index]; }
                var newComponent = new Component();
                _allLibraries.Add(index, newComponent);
                return newComponent;
            }
        }
        private BrowserType _requestingBrowser;
        private BrowserType RequestingBrowser
        {
            get
            {
                if (_requestingBrowser == 0)
                {
                    var browser = HttpContext.Current.Request.Browser.Type;
                    if (browser.Length > 2 && browser.Substring(0, 2) == "IE")
                    {
                        switch (browser[2])
                        {
                            case '6':
                                _requestingBrowser = BrowserType.Ie6;
                                break;
                            case '7':
                                _requestingBrowser = BrowserType.Ie7;
                                break;
                            case '8':
                                _requestingBrowser = BrowserType.Ie8;
                                break;
                            default:
                                _requestingBrowser = BrowserType.W3cCompliant;
                                break;
                        }
                    }
                    else
                    {
                        _requestingBrowser = BrowserType.W3cCompliant;
                    }
                }
                return _requestingBrowser;
            }
        }
        private bool IncludesCurrentBrowser(BrowserType browserType)
        {
            if (browserType == BrowserType.All) { return true; }
            return browserType.HasFlag(RequestingBrowser);
        }
    }
    public class ItemRegistrar
    {
        private readonly string _format;
        private readonly List<string> _items;
        private readonly UrlHelper _urlHelper;

        public ItemRegistrar(string format, UrlHelper urlHelper)
        {
            _format = format;
            _items = new List<string>();
            _urlHelper = urlHelper;
        }
        internal void Add(IEnumerable<string> urls)
        {
            foreach (string url in urls)
            {
                Add(url);
            }
        }
        public ItemRegistrar Add(string url)
        {
            url = _urlHelper.Content(url);
            if (!_items.Contains(url))
                { _items.Add( url); }
            return this;
        }
        public IHtmlString Render()
        {
            var sb = new StringBuilder();
            foreach (var item in _items)
            {
                var fmt = string.Format(_format, item);
                sb.AppendLine(fmt);
            }
            return new HtmlString(sb.ToString());
        }
    }
    public class ItemRegistrarFormatters
    {
        public const string StyleFormat = "<link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\" />";
        public const string ScriptFormat = "<script src=\"{0}\" type=\"text/javascript\"></script>";
    }
}