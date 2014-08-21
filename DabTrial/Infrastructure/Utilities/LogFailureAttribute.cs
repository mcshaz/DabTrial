﻿using Common.Logging;
using Hangfire.Common;
using Hangfire.States;
using Hangfire.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DabTrial.Infrastructure.Utilities
{
    public class LogFailureAttribute : JobFilterAttribute, IApplyStateFilter
    {
        private static readonly ILog Logger = LogManager.GetCurrentClassLogger();
        public void OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            var failedState = context.NewState as FailedState;
            if (failedState != null)
            {
                Logger.Error(
                String.Format("Background job #{0} was failed with an exception.", context.JobId),
                failedState.Exception);
            }
        }
        public void OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
        }
    }
}