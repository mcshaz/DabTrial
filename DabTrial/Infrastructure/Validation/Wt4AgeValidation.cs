
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DabTrial.Infrastructure.Validation
{
    public class Wt4AgeAttribute : ValidationAttribute, IClientValidatable
    {
        private const string DefaultErrorMessageFormatString = "The value for {0} is outside the extremes of weight for age";

        protected readonly string WeeksGestationAtBirthProperty;
        public double Zwarn { get; set; }
        protected readonly double Zfail;
        protected readonly string DobProperty;
        protected readonly string GenderProperty;
        protected readonly object MaleValue;

        public Wt4AgeAttribute(double zfail, string weeksGestationAtBirthProperty, string dobProperty, string genderProperty, object maleValue)
        {
            Zfail = zfail;
            DobProperty = dobProperty;
            WeeksGestationAtBirthProperty = weeksGestationAtBirthProperty;
            GenderProperty = genderProperty;
            MaleValue = maleValue;
        }

        protected override ValidationResult IsValid(object value,
                                                    ValidationContext validationContext)
        {
            object instance = validationContext.ObjectInstance;
            Type type = validationContext.ObjectType;
            DateTime? dOB = (DateTime?)type.GetProperty(DobProperty).GetValue(instance, null); ;
            double wt;
            string wtString = value as string;
            if (wtString == null)
            {
                wt = Convert.ToDouble(value);
            }
            else
            {
                wt = double.Parse(wtString);
            }
            int? gestationAtBirth = (int?)type.GetProperty(WeeksGestationAtBirthProperty).GetValue(instance, null);
            object genderVal = type.GetProperty(GenderProperty).GetValue(instance, null);
            if (genderVal == null)
            {
                return ValidationResult.Success; //rely on required attribute
            }
            if (!gestationAtBirth.HasValue)
            {
                if (!dOB.HasValue) { return ValidationResult.Success; } //rely on required attribute
                gestationAtBirth = 40; // default to term - if this default is not wanted, place required attribute on this field
            }
            int cga = (int)DateUtilities.CorrectedAgeInWeeks(dOB.Value, gestationAtBirth.Value);
            var z = stats.Weight4Age(wt, cga, genderVal.Equals(MaleValue));
            if (z < Zfail && z > -Zfail) { return ValidationResult.Success; }
            string errorMessage = GetErrorMessage(validationContext.DisplayName);
            return new ValidationResult(errorMessage);
        }
        protected string GetErrorMessage(string displayName)
        {
            return ErrorMessage ?? string.Format(DefaultErrorMessageFormatString,
                                                    displayName);
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata,
                                                                                ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = ErrorMessage ?? GetErrorMessage(metadata.GetDisplayName()),
                ValidationType = "wtforage",
            }; //"zwarn", "zfail", "dobprop", "weeksgestationprop","genderprop","maleval"
            rule.ValidationParameters.Add("zfail", Zfail);
            if (Zwarn != 0) { rule.ValidationParameters.Add("zwarn", Zwarn); }
            rule.ValidationParameters.Add("maleval", MaleValue);
            rule.ValidationParameters.Add("weeksgestationprop", "*." + WeeksGestationAtBirthProperty); // for edge cases displaying in heirarchical form
            rule.ValidationParameters.Add("genderprop", "*." + GenderProperty);
            rule.ValidationParameters.Add("dobprop", "*." + DobProperty);
            yield return rule;
        }
    }
    internal class Distribution
    {
        public double L {get; set;}
        public double M {get; set;}
        public double S {get; set;}
    }
    public static class stats
    {
        private static Distribution FemaleWtParams(int ageMonths)
        {
            switch (ageMonths)
            {
                case 0: return new Distribution { L = 1.5091874599456787, M = 3.399186372756958, S = .1421067267656326 };
                case 1: return new Distribution { L = 1.3579443693161013, M = 3.7975285053253174, S = .1380759179592133 };
                case 2: return new Distribution { L = 1.1055376529693604, M = 4.544776439666748, S = .1317338943481445 };
                case 3: return new Distribution { L = .902596652507782, M = 5.2305841445922852, S = .126892700791359 };
                case 4: return new Distribution { L = .7341214418411255, M = 5.8599610328674316, S = .1230251789093018 };
                case 5: return new Distribution { L = .5902352929115295, M = 6.4375877380371094, S = .1198409125208855 };
                case 6: return new Distribution { L = .4643915593624115, M = 6.9678506851196289, S = .1171668693423271 };
                case 7: return new Distribution { L = .3521640598773956, M = 7.4548540115356445, S = .114893838763237 };
                case 8: return new Distribution { L = .2504978775978088, M = 7.9024362564086914, S = .1129496470093727 };
                case 9: return new Distribution { L = .1572475135326386, M = 8.314178466796875, S = .1112846881151199 };
                case 10: return new Distribution { L = .0708857253193855, M = 8.6934185028076172, S = .109863705933094 };
                case 11: return new Distribution { L = -0.0096849296242, M = 9.0432615280151367, S = .1086607798933983 };
                case 12: return new Distribution { L = -0.08525799959898, M = 9.3665933609008789, S = .1076562106609344 };
                case 13: return new Distribution { L = -0.156409457325935, M = 9.6660890579223633, S = .1068345159292221 };
                case 14: return new Distribution { L = -0.223558694124222, M = 9.9442262649536133, S = .1061830818653107 };
                case 15: return new Distribution { L = -0.287013471126556, M = 10.203293800354004, S = .1056912392377853 };
                case 16: return new Distribution { L = -0.346999198198318, M = 10.445405960083008, S = .1053496301174164 };
                case 17: return new Distribution { L = -0.403689175844193, M = 10.672507286071776, S = .1051497533917427 };
                case 18: return new Distribution { L = -0.457218766212463, M = 10.886385917663574, S = .105083666741848 };
                case 19: return new Distribution { L = -0.507700741291046, M = 11.088681221008301, S = .1051437556743622 };
                case 20: return new Distribution { L = -0.555235981941223, M = 11.280895233154297, S = .105322577059269 };
                case 21: return new Distribution { L = -0.599921107292175, M = 11.464397430419922, S = .1056127771735191 };
                case 22: return new Distribution { L = -0.641854166984558, M = 11.640434265136721, S = .1060070246458054 };
                case 23: return new Distribution { L = -0.681138098239899, M = 11.810138702392578, S = .1064979583024979 };
                case 24: return new Distribution { L = -0.717882812023163, M = 11.97453784942627, S = .1070781946182251 };
                case 25: return new Distribution { L = -0.752206146717072, M = 12.134554862976074, S = .1077403426170349 };
                case 26: return new Distribution { L = -0.784233570098877, M = 12.291025161743164, S = .1084770113229752 };
                case 27: return new Distribution { L = -0.814095795154572, M = 12.444692611694336, S = .1092808246612549 };
                case 28: return new Distribution { L = -0.841935515403748, M = 12.59622287750244, S = .1101444885134697 };
                case 29: return new Distribution { L = -0.867889404296875, M = 12.746209144592283, S = .1110608130693436 };
                case 30: return new Distribution { L = -0.892102658748627, M = 12.895172119140623, S = .1120227575302124 };
                case 31: return new Distribution { L = -0.914718806743622, M = 13.043571472167969, S = .1130234673619271 };
                case 32: return new Distribution { L = -0.935879647731781, M = 13.191808700561523, S = .1140563189983368 };
                case 33: return new Distribution { L = -0.955723464488983, M = 13.340229034423828, S = .1151149496436119 };
                case 34: return new Distribution { L = -0.974381029605865, M = 13.489133834838867, S = .1161933392286301 };
                case 35: return new Distribution { L = -0.991980731487274, M = 13.638774871826172, S = .1172857508063316 };
                case 36: return new Distribution { L = -1.008640766143799, M = 13.789365768432615, S = .1183868497610092 };
                case 37: return new Distribution { L = -1.024471282958984, M = 13.941082954406738, S = .1194916665554047 };
                case 38: return new Distribution { L = -1.039573550224304, M = 14.094071388244627, S = .1205956563353538 };
                case 39: return new Distribution { L = -1.054039478302002, M = 14.24844455718994, S = .1216946765780449 };
                case 40: return new Distribution { L = -1.067946791648865, M = 14.404292106628418, S = .1227850317955017 };
                case 41: return new Distribution { L = -1.081374168395996, M = 14.561675071716309, S = .1238633990287781 };
                case 42: return new Distribution { L = -1.094381451606751, M = 14.720640182495115, S = .1249269396066666 };
                case 43: return new Distribution { L = -1.107021570205689, M = 14.881213188171388, S = .1259732246398926 };
                case 44: return new Distribution { L = -1.119338750839233, M = 15.043405532836914, S = .1270002126693726 };
                case 45: return new Distribution { L = -1.131367802619934, M = 15.20721435546875, S = .1280062943696976 };
                case 46: return new Distribution { L = -1.143135905265808, M = 15.372627258300779, S = .1289902180433273 };
                case 47: return new Distribution { L = -1.154662132263184, M = 15.539622306823732, S = .129951149225235 };
                case 48: return new Distribution { L = -1.165958404541016, M = 15.708169937133787, S = .1308885216712952 };
                case 49: return new Distribution { L = -1.177029967308044, M = 15.878236770629885, S = .1318021863698959 };
                case 50: return new Distribution { L = -1.187870979309082, M = 16.049783706665039, S = .1326922625303268 };
                case 51: return new Distribution { L = -1.198484063148498, M = 16.222770690917969, S = .1335591077804565 };
                case 52: return new Distribution { L = -1.208853960037232, M = 16.397153854370117, S = .1344033926725388 };
                case 53: return new Distribution { L = -1.21896505355835, M = 16.572891235351563, S = .1352259963750839 };
                case 54: return new Distribution { L = -1.228798270225525, M = 16.749942779541016, S = .1360280066728592 };
                case 55: return new Distribution { L = -1.238330841064453, M = 16.928266525268555, S = .1368107348680496 };
                case 56: return new Distribution { L = -1.247537970542908, M = 17.107826232910156, S = .1375756114721298 };
                case 57: return new Distribution { L = -1.256392121315002, M = 17.288589477539063, S = .138324186205864 };
                case 58: return new Distribution { L = -1.264864802360535, M = 17.470523834228516, S = .1390581876039505 };
                case 59: return new Distribution { L = -1.272925972938538, M = 17.653606414794922, S = .1397793889045715 };
                case 60: return new Distribution { L = -1.280545115470886, M = 17.837818145751953, S = .1404896378517151 };
                case 61: return new Distribution { L = -1.287691473960877, M = 18.023139953613281, S = .141190841794014 };
                case 62: return new Distribution { L = -1.294332027435303, M = 18.209564208984375, S = .1418849676847458 };
                case 63: return new Distribution { L = -1.300441503524781, M = 18.397087097167969, S = .1425739377737045 };
                case 64: return new Distribution { L = -1.305989027023315, M = 18.585712432861328, S = .1432597041130066 };
                case 65: return new Distribution { L = -1.310946941375732, M = 18.775447845458984, S = .1439442187547684 };
                case 66: return new Distribution { L = -1.315289497375489, M = 18.966306686401367, S = .1446293592453003 };
                case 67: return new Distribution { L = -1.318992972373962, M = 19.15831184387207, S = .1453169882297516 };
                case 68: return new Distribution { L = -1.322035312652588, M = 19.351491928100582, S = .1460089087486267 };
                case 69: return new Distribution { L = -1.324398159980774, M = 19.545877456665039, S = .1467068195343018 };
                case 70: return new Distribution { L = -1.326064586639404, M = 19.741508483886719, S = .1474123597145081 };
                case 71: return new Distribution { L = -1.327020406723023, M = 19.938430786132812, S = .1481271088123322 };
                case 72: return new Distribution { L = -1.327256441116333, M = 20.136695861816406, S = .1488524824380875 };
                case 73: return new Distribution { L = -1.326763868331909, M = 20.336359024047852, S = .1495898365974426 };
                case 74: return new Distribution { L = -1.325538635253907, M = 20.537483215332031, S = .1503403931856155 };
                case 75: return new Distribution { L = -1.323579668998719, M = 20.740133285522461, S = .1511052697896957 };
                case 76: return new Distribution { L = -1.320888042449951, M = 20.944379806518555, S = .1518854647874832 };
                case 77: return new Distribution { L = -1.317468643188477, M = 21.150300979614254, S = .1526818126440048 };
                case 78: return new Distribution { L = -1.313331484794617, M = 21.357973098754883, S = .1534950435161591 };
                case 79: return new Distribution { L = -1.308487057685852, M = 21.567480087280273, S = .1543257534503937 };
                case 80: return new Distribution { L = -1.302948117256165, M = 21.778909683227539, S = .1551744192838669 };
                case 81: return new Distribution { L = -1.296733856201172, M = 21.992347717285156, S = .1560413241386414 };
                case 82: return new Distribution { L = -1.289863348007202, M = 22.2078857421875, S = .1569266617298126 };
                case 83: return new Distribution { L = -1.282358765602112, M = 22.425617218017575, S = .1578305065631867 };
                case 84: return new Distribution { L = -1.274244904518127, M = 22.645637512207031, S = .1587527394294739 };
                case 85: return new Distribution { L = -1.265548825263977, M = 22.8680419921875, S = .1596931666135788 };
                case 86: return new Distribution { L = -1.256299376487732, M = 23.092926025390625, S = .1606514155864716 };
                case 87: return new Distribution { L = -1.246530652046204, M = 23.320384979248047, S = .1616269499063492 };
                case 88: return new Distribution { L = -1.236266851425171, M = 23.550518035888672, S = .1626193076372147 };
                case 89: return new Distribution { L = -1.225551366806031, M = 23.783416748046875, S = .1636275947093964 };
                case 90: return new Distribution { L = -1.214410901069641, M = 24.019176483154297, S = .164651095867157 };
                case 91: return new Distribution { L = -1.202884435653686, M = 24.257890701293945, S = .1656888127326965 };
                case 92: return new Distribution { L = -1.191007852554321, M = 24.49964714050293, S = .1667396575212479 };
                case 93: return new Distribution { L = -1.178818583488464, M = 24.744535446166992, S = .1678024977445602 };
                case 94: return new Distribution { L = -1.166354417800903, M = 24.992637634277344, S = .1688760370016098 };
                case 95: return new Distribution { L = -1.153653740882874, M = 25.244033813476559, S = .1699589192867279 };
                case 96: return new Distribution { L = -1.140751361846924, M = 25.498802185058594, S = .1710497587919235 };
                case 97: return new Distribution { L = -1.127684116363525, M = 25.757017135620117, S = .1721470355987549 };
                case 98: return new Distribution { L = -1.114490270614624, M = 26.018743515014648, S = .1732491850852966 };
                case 99: return new Distribution { L = -1.101204872131348, M = 26.284042358398441, S = .1743545681238174 };
                case 100: return new Distribution { L = -1.087863445281982, M = 26.552974700927734, S = .1754615157842636 };
                case 101: return new Distribution { L = -1.074500918388367, M = 26.825588226318359, S = .1765682846307755 };
                case 102: return new Distribution { L = -1.061151266098023, M = 27.1019287109375, S = .1776731312274933 };
                case 103: return new Distribution { L = -1.047847151756287, M = 27.382034301757813, S = .1787742376327515 };
                case 104: return new Distribution { L = -1.034620523452759, M = 27.665933609008789, S = .1798698306083679 };
                case 105: return new Distribution { L = -1.021502256393433, M = 27.953653335571289, S = .1809580624103546 };
                case 106: return new Distribution { L = -1.008521676063538, M = 28.245204925537109, S = .1820371150970459 };
                case 107: return new Distribution { L = -0.995707511901855, M = 28.54060173034668, S = .1831051707267761 };
                case 108: return new Distribution { L = -0.983086824417114, M = 28.839839935302734, S = .1841604113578796 };
                case 109: return new Distribution { L = -0.97068578004837, M = 29.142911911010746, S = .185201033949852 };
                case 110: return new Distribution { L = -0.95852917432785, M = 29.449802398681641, S = .1862252801656723 };
                case 111: return new Distribution { L = -0.94664055109024, M = 29.76048469543457, S = .1872314214706421 };
                case 112: return new Distribution { L = -0.935042440891266, M = 30.074926376342773, S = .1882177293300629 };
                case 113: return new Distribution { L = -0.923756062984467, M = 30.393081665039062, S = .1891825497150421 };
                case 114: return new Distribution { L = -0.912801444530487, M = 30.714900970458984, S = .1901242882013321 };
                case 115: return new Distribution { L = -0.902197659015656, M = 31.040321350097656, S = .1910413801670075 };
                case 116: return new Distribution { L = -0.89196252822876, M = 31.369274139404297, S = .1919323205947876 };
                case 117: return new Distribution { L = -0.882112920284271, M = 31.701681137084961, S = .1927956789731979 };
                case 118: return new Distribution { L = -0.8726646900177, M = 32.037448883056641, S = .1936300992965699 };
                case 119: return new Distribution { L = -0.86363273859024, M = 32.376487731933594, S = .194434255361557 };
                case 120: return new Distribution { L = -0.855031073093414, M = 32.718681335449219, S = .195206955075264 };
                case 121: return new Distribution { L = -0.846872806549072, M = 33.063922882080078, S = .1959470063447952 };
                case 122: return new Distribution { L = -0.839170217514038, M = 33.412086486816406, S = .1966533660888672 };
                case 123: return new Distribution { L = -0.831934928894043, M = 33.763034820556641, S = .1973250210285187 };
                case 124: return new Distribution { L = -0.825177669525147, M = 34.116630554199219, S = .1979610621929169 };
                case 125: return new Distribution { L = -0.818908751010895, M = 34.472721099853516, S = .1985606551170349 };
                case 126: return new Distribution { L = -0.813137650489807, M = 34.831153869628906, S = .1991230398416519 };
                case 127: return new Distribution { L = -0.807873427867889, M = 35.191761016845703, S = .199647530913353 };
                case 128: return new Distribution { L = -0.803122639656067, M = 35.554370880126953, S = .2001335918903351 };
                case 129: return new Distribution { L = -0.798897683620453, M = 35.918800354003906, S = .2005806118249893 };
                case 130: return new Distribution { L = -0.795203506946564, M = 36.284862518310547, S = .2009882181882858 };
                case 131: return new Distribution { L = -0.79204797744751, M = 36.652362823486328, S = .2013560235500336 };
                case 132: return new Distribution { L = -0.789435267448425, M = 37.021106719970703, S = .2016837894916534 };
                case 133: return new Distribution { L = -0.787374436855316, M = 37.390888214111328, S = .2019712775945664 };
                case 134: return new Distribution { L = -0.785870671272278, M = 37.761489868164063, S = .2022183686494827 };
                case 135: return new Distribution { L = -0.784929871559143, M = 38.132698059082031, S = .2024250030517578 };
                case 136: return new Distribution { L = -0.784557580947876, M = 38.504295349121094, S = .2025911808013916 };
                case 137: return new Distribution { L = -0.784759163856506, M = 38.876056671142578, S = .2027169764041901 };
                case 138: return new Distribution { L = -0.78553968667984, M = 39.247745513916016, S = .2028025388717651 };
                case 139: return new Distribution { L = -0.786904096603394, M = 39.619140625, S = .2028480470180512 };
                case 140: return new Distribution { L = -0.788858234882355, M = 39.990001678466797, S = .2028537541627884 };
                case 141: return new Distribution { L = -0.79140305519104, M = 40.360092163085938, S = .2028200477361679 };
                case 142: return new Distribution { L = -0.794546365737915, M = 40.729175567626953, S = .2027472406625748 };
                case 143: return new Distribution { L = -0.798291027545929, M = 41.097011566162109, S = .2026357650756836 };
                case 144: return new Distribution { L = -0.802640914916992, M = 41.463359832763672, S = .2024860978126526 };
                case 145: return new Distribution { L = -0.807599604129791, M = 41.827980041503906, S = .2022987902164459 };
                case 146: return new Distribution { L = -0.813170433044434, M = 42.190631866455078, S = .2020743787288666 };
                case 147: return new Distribution { L = -0.819356679916382, M = 42.551082611083984, S = .2018135190010071 };
                case 148: return new Distribution { L = -0.826161205768585, M = 42.909088134765625, S = .2015168517827988 };
                case 149: return new Distribution { L = -0.833586037158966, M = 43.264415740966797, S = .2011850774288178 };
                case 150: return new Distribution { L = -0.841634929180145, M = 43.616832733154297, S = .2008189260959625 };
                case 151: return new Distribution { L = -0.850307464599609, M = 43.966117858886719, S = .2004192024469376 };
                case 152: return new Distribution { L = -0.859607517719269, M = 44.312034606933594, S = .199986681342125 };
                case 153: return new Distribution { L = -0.869534313678741, M = 44.654373168945313, S = .1995222270488739 };
                case 154: return new Distribution { L = -0.880088627338409, M = 44.992912292480469, S = .1990267336368561 };
                case 155: return new Distribution { L = -0.891270577907562, M = 45.327445983886719, S = .1985010951757431 };
                case 156: return new Distribution { L = -0.903079450130463, M = 45.657768249511719, S = .1979462504386902 };
                case 157: return new Distribution { L = -0.915513515472412, M = 45.983688354492195, S = .1973631978034973 };
                case 158: return new Distribution { L = -0.928569436073303, M = 46.305007934570313, S = .1967529356479645 };
                case 159: return new Distribution { L = -0.942245841026306, M = 46.621551513671875, S = .1961164772510529 };
                case 160: return new Distribution { L = -0.956537902355194, M = 46.933143615722656, S = .1954548954963684 };
                case 161: return new Distribution { L = -0.971440494060516, M = 47.239620208740234, S = .1947692781686783 };
                case 162: return new Distribution { L = -0.986947298049927, M = 47.540824890136719, S = .1940607577562332 };
                case 163: return new Distribution { L = -1.003050923347473, M = 47.836616516113281, S = .1933304816484451 };
                case 164: return new Distribution { L = -1.019742369651794, M = 48.126850128173821, S = .1925796121358872 };
                case 165: return new Distribution { L = -1.037011742591858, M = 48.411411285400391, S = .1918093711137772 };
                case 166: return new Distribution { L = -1.054847002029419, M = 48.690177917480469, S = .1910209953784942 };
                case 167: return new Distribution { L = -1.073234796524048, M = 48.963047027587891, S = .1902157366275787 };
                case 168: return new Distribution { L = -1.092160224914551, M = 49.229934692382813, S = .1893949061632156 };
                case 169: return new Distribution { L = -1.111606121063232, M = 49.490753173828125, S = .1885598003864288 };
                case 170: return new Distribution { L = -1.131553769111633, M = 49.745441436767578, S = .1877118051052094 };
                case 171: return new Distribution { L = -1.151982069015503, M = 49.993942260742195, S = .1868522614240646 };
                case 172: return new Distribution { L = -1.172868132591248, M = 50.236209869384766, S = .1859826147556305 };
                case 173: return new Distribution { L = -1.19418466091156, M = 50.472221374511719, S = .1851043254137039 };
                case 174: return new Distribution { L = -1.215907454490662, M = 50.701957702636719, S = .1842188090085983 };
                case 175: return new Distribution { L = -1.23800528049469, M = 50.925411224365234, S = .1833275556564331 };
                case 176: return new Distribution { L = -1.260445594787598, M = 51.142593383789063, S = .1824321150779724 };
                case 177: return new Distribution { L = -1.283193588256836, M = 51.353527069091797, S = .1815340220928192 };
                case 178: return new Distribution { L = -1.306212067604065, M = 51.558246612548835, S = .1806348413228989 };
                case 179: return new Distribution { L = -1.329460978507996, M = 51.756805419921875, S = .1797361671924591 };
                case 180: return new Distribution { L = -1.352898001670837, M = 51.949256896972656, S = .1788396090269089 };
                case 181: return new Distribution { L = -1.37647819519043, M = 52.13568115234375, S = .1779468059539795 };
                case 182: return new Distribution { L = -1.4001544713974, M = 52.316162109375, S = .1770593822002411 };
                case 183: return new Distribution { L = -1.423876762390137, M = 52.490795135498047, S = .1761789917945862 };
                case 184: return new Distribution { L = -1.447593212127686, M = 52.659698486328125, S = .1753072887659073 };
                case 185: return new Distribution { L = -1.47124969959259, M = 52.822986602783203, S = .1744459569454193 };
                case 186: return new Distribution { L = -1.494789838790894, M = 52.980792999267578, S = .1735966354608536 };
                case 187: return new Distribution { L = -1.518155455589294, M = 53.133270263671875, S = .1727609783411026 };
                case 188: return new Distribution { L = -1.541286945343018, M = 53.280563354492195, S = .1719406396150589 };
                case 189: return new Distribution { L = -1.564122796058655, M = 53.422843933105469, S = .1711372286081314 };
                case 190: return new Distribution { L = -1.586600661277771, M = 53.560283660888672, S = .1703523695468903 };
                case 191: return new Distribution { L = -1.608657002449036, M = 53.693065643310547, S = .1695876121520996 };
                case 192: return new Distribution { L = -1.63022768497467, M = 53.821384429931641, S = .1688444912433624 };
                case 193: return new Distribution { L = -1.651248216629028, M = 53.945438385009766, S = .1681245416402817 };
                case 194: return new Distribution { L = -1.671653866767883, M = 54.065433502197266, S = .1674291789531708 };
                case 195: return new Distribution { L = -1.691380620002747, M = 54.181583404541016, S = .1667598187923431 };
                case 196: return new Distribution { L = -1.710364580154419, M = 54.294113159179687, S = .166117787361145 };
                case 197: return new Distribution { L = -1.728543162345886, M = 54.403244018554687, S = .1655043661594391 };
                case 198: return new Distribution { L = -1.745855331420898, M = 54.509208679199219, S = .1649207472801209 };
                case 199: return new Distribution { L = -1.762241244316101, M = 54.612236022949219, S = .1643680483102799 };
                case 200: return new Distribution { L = -1.777643799781799, M = 54.712566375732422, S = .1638473272323608 };
                case 201: return new Distribution { L = -1.792007923126221, M = 54.810440063476562, S = .1633594930171967 };
                case 202: return new Distribution { L = -1.805281639099121, M = 54.906097412109375, S = .1629054099321365 };
                case 203: return new Distribution { L = -1.817416310310364, M = 54.999778747558594, S = .1624858379364014 };
                case 204: return new Distribution { L = -1.828366756439209, M = 55.091720581054688, S = .1621014028787613 };
                case 205: return new Distribution { L = -1.838091611862182, M = 55.182167053222656, S = .1617526412010193 };
                case 206: return new Distribution { L = -1.846554040908814, M = 55.271350860595703, S = .1614399403333664 };
                case 207: return new Distribution { L = -1.853721737861633, M = 55.359504699707031, S = .1611636281013489 };
                case 208: return new Distribution { L = -1.859567284584046, M = 55.446853637695313, S = .1609238386154175 };
                case 209: return new Distribution { L = -1.864068388938904, M = 55.533622741699219, S = .1607206016778946 };
                case 210: return new Distribution { L = -1.867208600044251, M = 55.620014190673821, S = .1605538427829742 };
                case 211: return new Distribution { L = -1.868976831436157, M = 55.70623779296875, S = .1604233235120773 };
                case 212: return new Distribution { L = -1.869371175765991, M = 55.79248046875, S = .1603285819292069 };
                case 213: return new Distribution { L = -1.868386507034302, M = 55.878925323486328, S = .1602692306041718 };
                case 214: return new Distribution { L = -1.866033911705017, M = 55.965728759765625, S = .1602445542812347 };
                case 215: return new Distribution { L = -1.862327814102173, M = 56.053047180175781, S = .160253718495369 };
                case 216: return new Distribution { L = -1.85728919506073, M = 56.140998840332031, S = .160295769572258 };
                case 217: return new Distribution { L = -1.850946307182312, M = 56.229694366455078, S = .1603695899248123 };
                case 218: return new Distribution { L = -1.843334197998047, M = 56.319221496582031, S = .1604739278554916 };
                case 219: return new Distribution { L = -1.834495544433594, M = 56.409629821777344, S = .1606073826551437 };
                case 220: return new Distribution { L = -1.824479818344116, M = 56.500957489013672, S = .1607683748006821 };
                case 221: return new Distribution { L = -1.813344240188598, M = 56.59320068359375, S = .1609552502632141 };
                case 222: return new Distribution { L = -1.801153421401978, M = 56.686325073242195, S = .1611661612987518 };
                case 223: return new Distribution { L = -1.787979364395142, M = 56.780261993408203, S = .1613991558551788 };
                case 224: return new Distribution { L = -1.7739018201828, M = 56.874904632568359, S = .1616521626710892 };
                case 225: return new Distribution { L = -1.759007692337036, M = 56.970100402832031, S = .1619229912757874 };
                case 226: return new Distribution { L = -1.743391633033752, M = 57.065650939941406, S = .1622093915939331 };
                case 227: return new Distribution { L = -1.727155447006226, M = 57.16131591796875, S = .1625090092420578 };
                case 228: return new Distribution { L = -1.710410714149475, M = 57.256797790527344, S = .1628193557262421 };
                case 229: return new Distribution { L = -1.693267107009888, M = 57.351757049560547, S = .1631381213665008 };
                case 230: return new Distribution { L = -1.675854444503784, M = 57.445781707763672, S = .1634627133607864 };
                case 231: return new Distribution { L = -1.658302903175354, M = 57.538402557373047, S = .1637906879186631 };
                case 232: return new Distribution { L = -1.640747427940369, M = 57.629100799560547, S = .1641195714473724 };
                case 233: return new Distribution { L = -1.623332858085632, M = 57.717277526855469, S = .1644469946622849 };
                case 234: return new Distribution { L = -1.606209397315979, M = 57.802265167236328, S = .164770632982254 };
                case 235: return new Distribution { L = -1.58953332901001, M = 57.883335113525391, S = .1650882959365845 };
                case 236: return new Distribution { L = -1.573467254638672, M = 57.959674835205078, S = .1653978824615479 };
                case 237: return new Distribution { L = -1.558179140090942, M = 58.0303955078125, S = .1656975001096726 };
                case 238: return new Distribution { L = -1.543846249580383, M = 58.094532012939453, S = .1659853905439377 };
                case 239: return new Distribution { L = -1.530642509460449, M = 58.151035308837891, S = .1662601083517075 };
                case 240: return new Distribution { L = -1.518754005432129, M = 58.198772430419922, S = .1665203720331192 };
                case 241: return new Distribution { L = -1.513361811637878, M = 58.218971252441406, S = .1666447520256043 };
                default: throw new ArgumentException("No data available");
            }
        }
        private static Distribution MaleWtParams(int ageMonths)
        {
            switch (ageMonths)
            {
                case 0: return new Distribution { L = 1.81515109539032, M = 3.530203104019165, S = .1523852795362473 };
                case 1: return new Distribution { L = 1.5475231409072876, M = 4.0031065940856934, S = .1460250169038773 };
                case 2: return new Distribution { L = 1.0687955617904663, M = 4.8795251846313477, S = .1364787667989731 };
                case 3: return new Distribution { L = .6959735155105591, M = 5.6728887557983398, S = .1296775043010712 };
                case 4: return new Distribution { L = .4198150932788849, M = 6.3913917541503906, S = .1247170865535736 };
                case 5: return new Distribution { L = .2198667973279953, M = 7.0418362617492676, S = .1210401207208633 };
                case 6: return new Distribution { L = .0775055959820747, M = 7.630424976348877, S = .1182712018489838 };
                case 7: return new Distribution { L = -0.021907610818744, M = 8.1629514694213867, S = .1161536946892738 };
                case 8: return new Distribution { L = -0.089440897107124, M = 8.6448326110839844, S = .1145103499293327 };
                case 9: return new Distribution { L = -0.133409097790718, M = 9.0811195373535156, S = .1132171601057053 };
                case 10: return new Distribution { L = -0.160095393657684, M = 9.4765005111694336, S = .1121862381696701 };
                case 11: return new Distribution { L = -0.174296855926514, M = 9.8353080749511719, S = .1113545373082161 };
                case 12: return new Distribution { L = -0.179718896746635, M = 10.161535263061523, S = .1106764152646065 };
                case 13: return new Distribution { L = -0.179253995418549, M = 10.458853721618652, S = .1101186349987984 };
                case 14: return new Distribution { L = -0.175184473395348, M = 10.730625152587892, S = .1096569374203682 };
                case 15: return new Distribution { L = -0.169322684407234, M = 10.979925155639648, S = .1092736497521401 };
                case 16: return new Distribution { L = -0.163113906979561, M = 11.209555625915527, S = .1089559569954872 };
                case 17: return new Distribution { L = -0.157709985971451, M = 11.422067642211914, S = .1086946800351143 };
                case 18: return new Distribution { L = -0.154022783041, M = 11.619776725769045, S = .1084833219647408 };
                case 19: return new Distribution { L = -0.152762144804001, M = 11.804779052734377, S = .1083174124360084 };
                case 20: return new Distribution { L = -0.154466584324837, M = 11.97896671295166, S = .1081939414143563 };
                case 21: return new Distribution { L = -0.159522026777267, M = 12.14404296875, S = .1081109568476677 };
                case 22: return new Distribution { L = -0.168179258704185, M = 12.301541328430176, S = .108067236840725 };
                case 23: return new Distribution { L = -0.180566802620888, M = 12.452830314636232, S = .1080620810389519 };
                case 24: return new Distribution { L = -0.19670195877552, M = 12.599135398864746, S = .1080950796604156 };
                case 25: return new Distribution { L = -0.216501206159592, M = 12.741543769836426, S = .1081660091876984 };
                case 26: return new Distribution { L = -0.239790484309196, M = 12.881022453308105, S = .1082747057080269 };
                case 27: return new Distribution { L = -0.266315847635269, M = 13.018424034118652, S = .1084210276603699 };
                case 28: return new Distribution { L = -0.295754969120026, M = 13.154496192932127, S = .1086047664284706 };
                case 29: return new Distribution { L = -0.327729374170303, M = 13.289896965026855, S = .10882568359375 };
                case 30: return new Distribution { L = -0.361817449331284, M = 13.425193786621094, S = .1090834215283394 };
                case 31: return new Distribution { L = -0.397568076848984, M = 13.560880661010742, S = .1093775779008865 };
                case 32: return new Distribution { L = -0.434520244598389, M = 13.697378158569336, S = .1097076460719109 };
                case 33: return new Distribution { L = -0.472188740968704, M = 13.83504581451416, S = .1100730821490288 };
                case 34: return new Distribution { L = -0.510123074054718, M = 13.97418212890625, S = .1104732379317284 };
                case 35: return new Distribution { L = -0.547885596752167, M = 14.115032196044922, S = .1109073981642723 };
                case 36: return new Distribution { L = -0.585070133209229, M = 14.257796287536619, S = .1113747879862785 };
                case 37: return new Distribution { L = -0.621319711208344, M = 14.402627944946287, S = .1118745133280754 };
                case 38: return new Distribution { L = -0.656296014785767, M = 14.549646377563477, S = .1124056875705719 };
                case 39: return new Distribution { L = -0.689735054969788, M = 14.698933601379396, S = .1129672527313232 };
                case 40: return new Distribution { L = -0.721410393714905, M = 14.850541114807127, S = .1135581135749817 };
                case 41: return new Distribution { L = -0.751175224781036, M = 15.004491806030272, S = .1141769587993622 };
                case 42: return new Distribution { L = -0.778904259204865, M = 15.160784721374512, S = .1148224845528603 };
                case 43: return new Distribution { L = -0.804515480995178, M = 15.319402694702148, S = .1154932901263237 };
                case 44: return new Distribution { L = -0.828003227710724, M = 15.480302810668944, S = .1161877736449242 };
                case 45: return new Distribution { L = -0.849380373954773, M = 15.6434326171875, S = .1169043034315109 };
                case 46: return new Distribution { L = -0.868699669837952, M = 15.808725357055664, S = .1176411509513855 };
                case 47: return new Distribution { L = -0.88603401184082, M = 15.976104736328123, S = .1183965429663658 };
                case 48: return new Distribution { L = -0.90150785446167, M = 16.145481109619141, S = .1191685572266579 };
                case 49: return new Distribution { L = -0.915241599082947, M = 16.316766738891602, S = .1199553236365318 };
                case 50: return new Distribution { L = -0.927377760410309, M = 16.489864349365234, S = .1207549124956131 };
                case 51: return new Distribution { L = -0.938069820404053, M = 16.664674758911133, S = .1215654239058495 };
                case 52: return new Distribution { L = -0.94747793674469, M = 16.841098785400391, S = .1223849281668663 };
                case 53: return new Distribution { L = -0.955765664577484, M = 17.019037246704102, S = .1232115626335144 };
                case 54: return new Distribution { L = -0.963096976280212, M = 17.198390960693359, S = .1240435019135475 };
                case 55: return new Distribution { L = -0.969633460044861, M = 17.379062652587891, S = .1248789951205254 };
                case 56: return new Distribution { L = -0.975532352924347, M = 17.560962677001953, S = .1257163435220718 };
                case 57: return new Distribution { L = -0.980937898159027, M = 17.744001388549805, S = .1265540271997452 };
                case 58: return new Distribution { L = -0.986006498336792, M = 17.928091049194336, S = .1273904591798782 };
                case 59: return new Distribution { L = -0.990866959095001, M = 18.113155364990231, S = .1282242983579636 };
                case 60: return new Distribution { L = -0.995644390583038, M = 18.299123764038089, S = .1290542781352997 };
                case 61: return new Distribution { L = -1.00045382976532, M = 18.485923767089844, S = .1298792511224747 };
                case 62: return new Distribution { L = -1.005399703979492, M = 18.673500061035156, S = .1306982189416885 };
                case 63: return new Distribution { L = -1.01057505607605, M = 18.861795425415039, S = .1315102428197861 };
                case 64: return new Distribution { L = -1.016061902046204, M = 19.050765991210937, S = .1323145925998688 };
                case 65: return new Distribution { L = -1.021931290626526, M = 19.24036979675293, S = .1331105977296829 };
                case 66: return new Distribution { L = -1.028242349624634, M = 19.430576324462891, S = .1338977515697479 };
                case 67: return new Distribution { L = -1.035043597221374, M = 19.621360778808594, S = .1346756666898727 };
                case 68: return new Distribution { L = -1.042372107505798, M = 19.812702178955082, S = .1354440897703171 };
                case 69: return new Distribution { L = -1.050254225730896, M = 20.004594802856445, S = .1362028568983078 };
                case 70: return new Distribution { L = -1.058705568313599, M = 20.197031021118164, S = .1369519531726837 };
                case 71: return new Distribution { L = -1.067731499671936, M = 20.390018463134769, S = .1376914829015732 };
                case 72: return new Distribution { L = -1.077321171760559, M = 20.583568572998047, S = .1384216696023941 };
                case 73: return new Distribution { L = -1.08747124671936, M = 20.777694702148441, S = .1391427665948868 };
                case 74: return new Distribution { L = -1.098152995109558, M = 20.97242546081543, S = .1398552358150482 };
                case 75: return new Distribution { L = -1.109334111213684, M = 21.167791366577148, S = .1405595988035202 };
                case 76: return new Distribution { L = -1.120974063873291, M = 21.36383056640625, S = .1412564963102341 };
                case 77: return new Distribution { L = -1.13302481174469, M = 21.560585021972656, S = .1419466137886047 };
                case 78: return new Distribution { L = -1.145431399345398, M = 21.75810432434082, S = .1426307857036591 };
                case 79: return new Distribution { L = -1.158132553100586, M = 21.956445693969727, S = .1433098912239075 };
                case 80: return new Distribution { L = -1.171061635017395, M = 22.155668258666992, S = .14398492872715 };
                case 81: return new Distribution { L = -1.184141993522644, M = 22.355838775634769, S = .1446569561958313 };
                case 82: return new Distribution { L = -1.197307229042053, M = 22.557022094726559, S = .1453270018100739 };
                case 83: return new Distribution { L = -1.210475087165832, M = 22.759296417236328, S = .1459962874650955 };
                case 84: return new Distribution { L = -1.223565220832825, M = 22.962734222412109, S = .1466660052537918 };
                case 85: return new Distribution { L = -1.236497282981873, M = 23.16741943359375, S = .1473373770713806 };
                case 86: return new Distribution { L = -1.249186277389526, M = 23.373434066772461, S = .148011714220047 };
                case 87: return new Distribution { L = -1.261555433273315, M = 23.580862045288089, S = .14869025349617 };
                case 88: return new Distribution { L = -1.273523569107056, M = 23.789791107177734, S = .1493742913007736 };
                case 89: return new Distribution { L = -1.285013794898987, M = 24.000310897827148, S = .1500651091337204 };
                case 90: return new Distribution { L = -1.295952081680298, M = 24.21251106262207, S = .1507639288902283 };
                case 91: return new Distribution { L = -1.306268453598023, M = 24.426481246948239, S = .1514719873666763 };
                case 92: return new Distribution { L = -1.315897583961487, M = 24.642311096191406, S = .1521904170513153 };
                case 93: return new Distribution { L = -1.32477879524231, M = 24.860095977783203, S = .1529203206300736 };
                case 94: return new Distribution { L = -1.332857608795166, M = 25.079923629760746, S = .1536627262830735 };
                case 95: return new Distribution { L = -1.340080142021179, M = 25.301885604858398, S = .1544186323881149 };
                case 96: return new Distribution { L = -1.346412062644959, M = 25.526069641113281, S = .1551887691020966 };
                case 97: return new Distribution { L = -1.351813316345215, M = 25.752565383911133, S = .1559739112854004 };
                case 98: return new Distribution { L = -1.356253981590271, M = 25.981460571289063, S = .1567746847867966 };
                case 99: return new Distribution { L = -1.359710812568665, M = 26.212839126586911, S = .1575915813446045 };
                case 100: return new Distribution { L = -1.362167119979858, M = 26.44679069519043, S = .1584249585866928 };
                case 101: return new Distribution { L = -1.363612413406372, M = 26.683395385742188, S = .1592750698328018 };
                case 102: return new Distribution { L = -1.364042162895203, M = 26.922735214233398, S = .1601419895887375 };
                case 103: return new Distribution { L = -1.363457798957825, M = 27.164892196655273, S = .1610256880521774 };
                case 104: return new Distribution { L = -1.361865639686585, M = 27.409944534301761, S = .1619259715080261 };
                case 105: return new Distribution { L = -1.359282612800598, M = 27.657970428466797, S = .1628424525260925 };
                case 106: return new Distribution { L = -1.355720520019531, M = 27.90904426574707, S = .1637747138738632 };
                case 107: return new Distribution { L = -1.351202487945557, M = 28.163242340087891, S = .1647221446037293 };
                case 108: return new Distribution { L = -1.345754384994507, M = 28.420637130737305, S = .1656839400529862 };
                case 109: return new Distribution { L = -1.339405417442322, M = 28.681299209594727, S = .1666592508554459 };
                case 110: return new Distribution { L = -1.332188129425049, M = 28.945301055908203, S = .1676470190286636 };
                case 111: return new Distribution { L = -1.324137449264526, M = 29.212705612182617, S = .1686460971832275 };
                case 112: return new Distribution { L = -1.315291047096252, M = 29.483585357666016, S = .1696552336215973 };
                case 113: return new Distribution { L = -1.305688261985779, M = 29.758001327514648, S = .1706730276346207 };
                case 114: return new Distribution { L = -1.295369863510132, M = 30.036020278930664, S = .1716979742050171 };
                case 115: return new Distribution { L = -1.284374952316284, M = 30.317703247070313, S = .1727285385131836 };
                case 116: return new Distribution { L = -1.272750854492188, M = 30.603111267089844, S = .1737629622220993 };
                case 117: return new Distribution { L = -1.260539174079895, M = 30.892301559448239, S = .1747994869947434 };
                case 118: return new Distribution { L = -1.247783660888672, M = 31.185329437255859, S = .1758362799882889 };
                case 119: return new Distribution { L = -1.234527707099915, M = 31.482254028320313, S = .1768714189529419 };
                case 120: return new Distribution { L = -1.220815062522888, M = 31.783123016357425, S = .1779029071331024 };
                case 121: return new Distribution { L = -1.206688404083252, M = 32.087989807128906, S = .1789287328720093 };
                case 122: return new Distribution { L = -1.192190170288086, M = 32.396903991699219, S = .1799468249082565 };
                case 123: return new Distribution { L = -1.177361726760865, M = 32.709907531738281, S = .1809550821781158 };
                case 124: return new Distribution { L = -1.162243843078613, M = 33.027042388916016, S = .1819513589143753 };
                case 125: return new Distribution { L = -1.146875977516174, M = 33.348350524902344, S = .1829335391521454 };
                case 126: return new Distribution { L = -1.131296515464783, M = 33.673870086669922, S = .183899462223053 };
                case 127: return new Distribution { L = -1.115542650222778, M = 34.003631591796875, S = .1848470121622086 };
                case 128: return new Distribution { L = -1.099650263786316, M = 34.337661743164063, S = .1857740432024002 };
                case 129: return new Distribution { L = -1.083654046058655, M = 34.675991058349609, S = .1866784691810608 };
                case 130: return new Distribution { L = -1.06758725643158, M = 35.018638610839844, S = .1875582337379456 };
                case 131: return new Distribution { L = -1.051482915878296, M = 35.365615844726563, S = .1884112805128098 };
                case 132: return new Distribution { L = -1.035367369651794, M = 35.716945648193359, S = .189235731959343 };
                case 133: return new Distribution { L = -1.019277334213257, M = 36.072624206542969, S = .1900295466184616 };
                case 134: return new Distribution { L = -1.003235340118408, M = 36.432659149169922, S = .1907909661531448 };
                case 135: return new Distribution { L = -0.987269878387451, M = 36.797042846679688, S = .1915182173252106 };
                case 136: return new Distribution { L = -0.971406638622284, M = 37.165767669677734, S = .1922096163034439 };
                case 137: return new Distribution { L = -0.955670118331909, M = 37.538814544677734, S = .1928635686635971 };
                case 138: return new Distribution { L = -0.940083861351013, M = 37.916156768798835, S = .1934785842895508 };
                case 139: return new Distribution { L = -0.924670219421387, M = 38.297771453857422, S = .1940532773733139 };
                case 140: return new Distribution { L = -0.909450829029083, M = 38.683612823486328, S = .1945863664150238 };
                case 141: return new Distribution { L = -0.89444625377655, M = 39.073642730712891, S = .1950767040252686 };
                case 142: return new Distribution { L = -0.879676282405853, M = 39.467807769775391, S = .1955232471227646 };
                case 143: return new Distribution { L = -0.865160048007965, M = 39.866039276123047, S = .1959250718355179 };
                case 144: return new Distribution { L = -0.850915968418121, M = 40.268276214599609, S = .1962814182043076 };
                case 145: return new Distribution { L = -0.836961925029755, M = 40.6744384765625, S = .1965916156768799 };
                case 146: return new Distribution { L = -0.82331520318985, M = 41.084434509277344, S = .1968551427125931 };
                case 147: return new Distribution { L = -0.809992730617523, M = 41.498172760009766, S = .1970716267824173 };
                case 148: return new Distribution { L = -0.797011137008667, M = 41.915546417236328, S = .1972407996654511 };
                case 149: return new Distribution { L = -0.784386694431305, M = 42.336441040039063, S = .1973625868558884 };
                case 150: return new Distribution { L = -0.772135496139526, M = 42.760730743408203, S = .1974370032548904 };
                case 151: return new Distribution { L = -0.760273516178131, M = 43.188285827636719, S = .1974642127752304 };
                case 152: return new Distribution { L = -0.748815953731537, M = 43.61895751953125, S = .1974445283412934 };
                case 153: return new Distribution { L = -0.737780392169952, M = 44.052593231201165, S = .1973783522844314 };
                case 154: return new Distribution { L = -0.727181553840637, M = 44.489028930664063, S = .1972662657499314 };
                case 155: return new Distribution { L = -0.717035472393036, M = 44.928092956542969, S = .1971089690923691 };
                case 156: return new Distribution { L = -0.707358360290527, M = 45.369602203369141, S = .1969072669744492 };
                case 157: return new Distribution { L = -0.69816642999649, M = 45.813362121582031, S = .196662113070488 };
                case 158: return new Distribution { L = -0.689476311206818, M = 46.259166717529297, S = .1963745355606079 };
                case 159: return new Distribution { L = -0.681304752826691, M = 46.706806182861328, S = .1960456967353821 };
                case 160: return new Distribution { L = -0.673668682575226, M = 47.156059265136719, S = .195676863193512 };
                case 161: return new Distribution { L = -0.666585206985474, M = 47.606689453125, S = .195269376039505 };
                case 162: return new Distribution { L = -0.660069942474365, M = 48.058464050292969, S = .1948247253894806 };
                case 163: return new Distribution { L = -0.654142618179321, M = 48.511131286621094, S = .1943444162607193 };
                case 164: return new Distribution { L = -0.6488196849823, M = 48.964431762695313, S = .1938300430774689 };
                case 165: return new Distribution { L = -0.64411860704422, M = 49.418102264404297, S = .1932833194732666 };
                case 166: return new Distribution { L = -0.640056788921356, M = 49.871875762939453, S = .192705973982811 };
                case 167: return new Distribution { L = -0.636651396751404, M = 50.325466156005859, S = .1920998096466065 };
                case 168: return new Distribution { L = -0.633919298648834, M = 50.778591156005859, S = .1914666742086411 };
                case 169: return new Distribution { L = -0.631876885890961, M = 51.230964660644531, S = .1908084750175476 };
                case 170: return new Distribution { L = -0.630539953708649, M = 51.682285308837891, S = .1901271045207977 };
                case 171: return new Distribution { L = -0.629923343658447, M = 52.132259368896477, S = .189424529671669 };
                case 172: return new Distribution { L = -0.630041062831879, M = 52.580585479736328, S = .1887027174234391 };
                case 173: return new Distribution { L = -0.630905747413635, M = 53.026954650878906, S = .1879636347293854 };
                case 174: return new Distribution { L = -0.632528483867645, M = 53.471065521240234, S = .187209278345108 };
                case 175: return new Distribution { L = -0.634918749332428, M = 53.912609100341797, S = .1864416301250458 };
                case 176: return new Distribution { L = -0.638083875179291, M = 54.351276397705078, S = .1856626570224762 };
                case 177: return new Distribution { L = -0.64202880859375, M = 54.786766052246094, S = .1848743259906769 };
                case 178: return new Distribution { L = -0.646755993366241, M = 55.218776702880859, S = .184078574180603 };
                case 179: return new Distribution { L = -0.652262270450592, M = 55.647010803222656, S = .1832773387432098 };
                case 180: return new Distribution { L = -0.658551633358002, M = 56.071163177490234, S = .1824724227190018 };
                case 181: return new Distribution { L = -0.665609002113342, M = 56.490959167480469, S = .1816657781600952 };
                case 182: return new Distribution { L = -0.6734259724617, M = 56.906108856201165, S = .1808591783046722 };
                case 183: return new Distribution { L = -0.681987285614014, M = 57.316341400146477, S = .1800543963909149 };
                case 184: return new Distribution { L = -0.691273629665375, M = 57.721389770507805, S = .1792531460523605 };
                case 185: return new Distribution { L = -0.701261043548584, M = 58.120998382568359, S = .1784571260213852 };
                case 186: return new Distribution { L = -0.711921095848084, M = 58.514923095703125, S = .1776679456233978 };
                case 187: return new Distribution { L = -0.723218500614166, M = 58.902931213378906, S = .1768871992826462 };
                case 188: return new Distribution { L = -0.735121190547943, M = 59.284797668457031, S = .1761163026094437 };
                case 189: return new Distribution { L = -0.747580409049988, M = 59.660327911376953, S = .1753568202257156 };
                case 190: return new Distribution { L = -0.760550677776337, M = 60.029315948486328, S = .1746100783348084 };
                case 191: return new Distribution { L = -0.773984551429749, M = 60.391586303710938, S = .1738773286342621 };
                case 192: return new Distribution { L = -0.787817716598511, M = 60.746986389160163, S = .1731599569320679 };
                case 193: return new Distribution { L = -0.801993072032928, M = 61.095367431640625, S = .1724590510129929 };
                case 194: return new Distribution { L = -0.816446423530579, M = 61.436599731445313, S = .1717757284641266 };
                case 195: return new Distribution { L = -0.831110298633575, M = 61.770572662353523, S = .171110987663269 };
                case 196: return new Distribution { L = -0.845914483070374, M = 62.097194671630859, S = .1704657524824143 };
                case 197: return new Distribution { L = -0.860786497592926, M = 62.416385650634766, S = .1698408722877502 };
                case 198: return new Distribution { L = -0.875652194023132, M = 62.728092193603523, S = .1692370623350143 };
                case 199: return new Distribution { L = -0.890436291694641, M = 63.032276153564453, S = .1686549782752991 };
                case 200: return new Distribution { L = -0.905063211917877, M = 63.32891845703125, S = .168095126748085 };
                case 201: return new Distribution { L = -0.919457495212555, M = 63.618015289306641, S = .1675579398870468 };
                case 202: return new Distribution { L = -0.93354469537735, M = 63.899585723876953, S = .1670437157154083 };
                case 203: return new Distribution { L = -0.947251737117767, M = 64.173667907714844, S = .1665526479482651 };
                case 204: return new Distribution { L = -0.960507869720459, M = 64.440322875976563, S = .1660847961902618 };
                case 205: return new Distribution { L = -0.973244786262512, M = 64.699615478515625, S = .1656400859355927 };
                case 206: return new Distribution { L = -0.985397517681122, M = 64.951644897460938, S = .1652183383703232 };
                case 207: return new Distribution { L = -0.996904790401459, M = 65.196533203125, S = .1648192405700684 };
                case 208: return new Distribution { L = -1.007705569267273, M = 65.434402465820313, S = .1644423753023148 };
                case 209: return new Distribution { L = -1.017756104469299, M = 65.665397644042969, S = .164087101817131 };
                case 210: return new Distribution { L = -1.027002692222595, M = 65.889701843261719, S = .1637527942657471 };
                case 211: return new Distribution { L = -1.035402297973633, M = 66.107490539550781, S = .1634386628866196 };
                case 212: return new Distribution { L = -1.042916297912598, M = 66.3189697265625, S = .1631438285112381 };
                case 213: return new Distribution { L = -1.049511909484863, M = 66.524368286132813, S = .1628673076629639 };
                case 214: return new Distribution { L = -1.055160760879517, M = 66.723907470703125, S = .1626080721616745 };
                case 215: return new Distribution { L = -1.059839963912964, M = 66.917839050292969, S = .1623650044202805 };
                case 216: return new Distribution { L = -1.063531994819641, M = 67.106422424316406, S = .162136971950531 };
                case 217: return new Distribution { L = -1.066223978996277, M = 67.289924621582031, S = .161922812461853 };
                case 218: return new Distribution { L = -1.067908883094788, M = 67.468635559082031, S = .1617213934659958 };
                case 219: return new Distribution { L = -1.068589925765991, M = 67.642814636230469, S = .1615315228700638 };
                case 220: return new Distribution { L = -1.06826114654541, M = 67.812767028808594, S = .1613523066043854 };
                case 221: return new Distribution { L = -1.066933751106262, M = 67.978775024414062, S = .1611827909946442 };
                case 222: return new Distribution { L = -1.064620971679688, M = 68.14111328125, S = .1610221862792969 };
                case 223: return new Distribution { L = -1.061341762542725, M = 68.300048828125, S = .1608699411153793 };
                case 224: return new Distribution { L = -1.057116985321045, M = 68.455848693847656, S = .1607257872819901 };
                case 225: return new Distribution { L = -1.051988959312439, M = 68.608718872070313, S = .1605895757675171 };
                case 226: return new Distribution { L = -1.045990347862244, M = 68.758895874023437, S = .1604616940021515 };
                case 227: return new Distribution { L = -1.039168238639832, M = 68.906532287597656, S = .1603429168462753 };
                case 228: return new Distribution { L = -1.031579613685608, M = 69.051765441894531, S = .1602344810962677 };
                case 229: return new Distribution { L = -1.023291945457459, M = 69.194671630859375, S = .1601381599903107 };
                case 230: return new Distribution { L = -1.014385104179382, M = 69.335273742675781, S = .16005639731884 };
                case 231: return new Distribution { L = -1.004952311515808, M = 69.4735107421875, S = .1599923372268677 };
                case 232: return new Distribution { L = -0.995101928710938, M = 69.609260559082031, S = .1599499881267548 };
                case 233: return new Distribution { L = -0.984958291053772, M = 69.742279052734375, S = .1599342375993729 };
                case 234: return new Distribution { L = -0.974663317203522, M = 69.872238159179688, S = .1599510014057159 };
                case 235: return new Distribution { L = -0.964376568794251, M = 69.998687744140625, S = .1600073873996735 };
                case 236: return new Distribution { L = -0.954274952411652, M = 70.121040344238281, S = .1601117700338364 };
                case 237: return new Distribution { L = -0.944551169872284, M = 70.238571166992188, S = .1602739244699478 };
                case 238: return new Distribution { L = -0.935410439968109, M = 70.350395202636719, S = .1605052053928375 };
                case 239: return new Distribution { L = -0.927059769630432, M = 70.455459594726562, S = .1608187854290008 };
                case 240: return new Distribution { L = -0.919718444347382, M = 70.552520751953125, S = .1612296104431152 };
                case 241: return new Distribution { L = -0.916487634181976, M = 70.597618103027344, S = .1614767909049988 };
                default: throw new ArgumentException("No data available");
            }
        }
        public static double Weight4Age(double weight, int cgaWeeks, bool isMale)
        {
            var stats = GetWeightStats(cgaWeeks, isMale);
            return ZfromParams(weight, stats.M, stats.S, stats.L);
        }
        private const double weeksPerMonth = 365.25/52;
        private static Distribution GetWeightStats(int cgaWeeks, bool isMale)
        {
            int lookupVal = (cgaWeeks<=40)
                ?0
                :(int)((cgaWeeks-40)/weeksPerMonth)+1;
            return isMale?MaleWtParams(lookupVal):FemaleWtParams(lookupVal);
        }
        public static double CumSnorm(Double Zscore)
        {
            double returnVal, ZAbs = Math.Abs(Zscore);
            if (ZAbs > 37)
            {
                return 0;
            }
            else
            {
                double Exponential = Math.Exp(-Math.Pow(ZAbs, 2) / 2);
                if (ZAbs < 7.07106781186547)
                {
                    double build = 3.52624965998911E-02 * ZAbs + 0.700383064443688;
                    build = build * ZAbs + 6.37396220353165;
                    build = build * ZAbs + 33.912866078383;
                    build = build * ZAbs + 112.079291497871;
                    build = build * ZAbs + 221.213596169931;
                    build = build * ZAbs + 220.206867912376;
                    returnVal = Exponential * build;
                    build = 8.83883476483184E-02 * ZAbs + 1.75566716318264;
                    build = build * ZAbs + 16.064177579207;
                    build = build * ZAbs + 86.7807322029461;
                    build = build * ZAbs + 296.564248779674;
                    build = build * ZAbs + 637.333633378831;
                    build = build * ZAbs + 793.826512519948;
                    build = build * ZAbs + 440.413735824752;
                    returnVal = returnVal / build;
                }
                else
                {
                    double build = ZAbs + 0.65;
                    build = ZAbs + 4 / build;
                    build = ZAbs + 3 / build;
                    build = ZAbs + 2 / build;
                    build = ZAbs + 1 / build;
                    returnVal = Exponential / build / 2.506628274631;
                }
            }
            return (Zscore < 0) ? returnVal : 1 - returnVal;
        }
        /// <summary>
        /// Returns the Z score for a value given statistical parameters
        /// </summary>
        /// <param name="xVal">measurement for evaluation</param>
        /// <param name="M">median</param>
        /// <param name="S">generalized coefficient of variation</param>
        /// <param name="L">power of box-cox transformation(optional)</param>
        /// <returns>Z</returns>
        public static double ZfromParams(double xVal, double M, double S, double L = 0)
        {
            if (L == 0)
            {
                return Math.Log(xVal / M) / S;
            }
            return (Math.Pow(xVal / M, L) - 1) / (L * S);
        }
        /// <summary>
        /// to obtain the value of a measurement for a particular Z score
        /// </summary>
        /// <param name="Z">Z score</param>
        /// <param name="M">median</param>
        /// <param name="S">generalized coefficient of variation</param>
        /// <param name="L">power of box-cox transformation(optional)</param>
        /// <returns>value of a measurement given statistical parameters </returns>
        public static double XfromParams(double Z, double M, double S, double L = 0)
        {
            if (L == 0)
            {
                return M * Math.Exp(S * Z);
            }
            else
            {
                return M * Math.Pow(1 + (L * S * Z), 1 / L);
            }
        }
        public static double InverseCumSnorm(double p)
        {
            // Coefficients in rational approximations
            double[] a = {-3.969683028665376e+01,  2.209460984245205e+02,
                                -2.759285104469687e+02,  1.383577518672690e+02,
                                -3.066479806614716e+01,  2.506628277459239e+00};

            double[] b = {-5.447609879822406e+01,  1.615858368580409e+02,
                                -1.556989798598866e+02,  6.680131188771972e+01,
                                -1.328068155288572e+01 };

            double[] c = {-7.784894002430293e-03, -3.223964580411365e-01,
                                -2.400758277161838e+00, -2.549732539343734e+00,
                                4.374664141464968e+00,  2.938163982698783e+00};

            double[] d =  {7.784695709041462e-03, 3.224671290700398e-01,
                                2.445134137142996e+00,  3.754408661907416e+00};

            // Define break-points.
            Double plow = 0.02425;
            Double phigh = 1 - plow;

            // Rational approximation for lower region:
            if (p < plow)
            {
                Double q = Math.Sqrt(-2 * Math.Log(p));
                return (((((c[0] * q + c[1]) * q + c[2]) * q + c[3]) * q + c[4]) * q + c[5]) /
                                                ((((d[0] * q + d[1]) * q + d[2]) * q + d[3]) * q + 1);
            }

            // Rational approximation for upper region:
            else if (phigh < p)
            {
                Double q = Math.Sqrt(-2 * Math.Log(1 - p));
                return -(((((c[0] * q + c[1]) * q + c[2]) * q + c[3]) * q + c[4]) * q + c[5]) /
                                                        ((((d[0] * q + d[1]) * q + d[2]) * q + d[3]) * q + 1);
            }
            else
            {
                // Rational approximation for central region:
                Double q = p - 0.5;
                Double r = q * q;
                return (((((a[0] * r + a[1]) * r + a[2]) * r + a[3]) * r + a[4]) * r + a[5]) * q /
                                            (((((b[0] * r + b[1]) * r + b[2]) * r + b[3]) * r + b[4]) * r + 1);
            }
        }
    }
}