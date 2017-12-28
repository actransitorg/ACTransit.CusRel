using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACTransit.CusRel.ServiceHost.VirusScan.Command;
using log4net;

namespace ACTransit.CusRel.ServiceHost.Request
{
    /// <summary>
    /// Allows requests, as commands, to return generic responses
    /// </summary>
    public class RequestManager
    {
        private static readonly ILog log = LogManager.GetLogger(nameof(RequestManager));
        public static T Execute<T>(object request) where T : class
        {
            log.Debug($"Begin Execute<{typeof(T).Name}>({request.GetType().Name})");
            try
            {

                if (typeof(T) == typeof(PostNewFiles))
                {
                    var commandRequest = request as RequestState;
                    if (commandRequest == null) throw new Exception("Expected parameter as RequestState.");
                    var command = new PostNewFiles(commandRequest);
                    try
                    {
                        command.Execute();
                    }
                    catch (Exception e)
                    {
                        log.Debug("PostNewFiles Exception", e);
                    }
                    if (command.ShouldRetry && command.Results != null)
                        foreach (var item in command.Results.Where(r => r.Code != null).ToList())
                            log.Debug($"id: {item.Id}, {item.Message}, {item.Warning})");
                    return command as T;
                }


                if (typeof(T) == typeof(ProcessScanReports))
                {
                    var commandRequest = request as RequestState;
                    if (commandRequest == null) throw new Exception("Expected parameter as RequestState.");
                    var command = new ProcessScanReports(commandRequest);
                    try
                    {
                        command.Execute();
                    }
                    catch (Exception e)
                    {
                        log.Debug("ProcessScanReports Exception", e);
                    }
                    if (command.ShouldRetry && command.Results != null)
                        foreach (var item in command.Results.Where(r => r.Code != null).ToList())
                            log.Debug($"id: {item.Id}, {item.Message}, {item.Warning})");
                    return command as T;
                }

                if (typeof(T) == typeof(ForceCloseFiles))
                {
                    var commandRequest = request as RequestState;
                    if (commandRequest == null) throw new Exception("Expected parameter as RequestState.");
                    var command = new ForceCloseFiles(commandRequest);
                    try
                    {
                        command.Execute();
                    }
                    catch (Exception e)
                    {
                        log.Debug("ForceCloseFiles Exception", e);
                    }
                    if (command.ShouldRetry && command.Results != null)
                        foreach (var item in command.Results.Where(r => r.Code != null).ToList())
                            log.Debug($"id: {item.Id}, {item.Message}, {item.Warning})");
                    return command as T;
                }










            }
            finally
            {
                log.Debug($"End Execute<{typeof(T).Name}>({request.GetType().Name})");
            }

            return null;
        }
    }
}
