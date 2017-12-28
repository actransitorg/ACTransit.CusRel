using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.Common
{
    /// <summary>
    ///     Response result, including OK, StatusCode and list of Errors
    /// </summary>
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class Result
    {
        public Result()
        {
            SetOK();
        }

        public Result(string id)
        {
            SetOK();
            Id = id;
        }

        /// <summary>
        ///     Is request and result valid and successful?
        /// </summary>
        [DataMember]
        public bool OK { get; set; }

        /// <summary>
        ///     Status code of result
        /// </summary>
        [DataMember]
        public int? StatusCode { get; set; }

        /// <summary>
        ///     List of errors
        /// </summary>
        [DataMember]
        public List<string> Errors { get; set; }

        /// <summary>
        ///     Generic return Id
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        public void SetOK()
        {
            OK = true;
        }

        public void SetFail(string Error)
        {
            OK = false;
            if (Errors == null)
                Errors = new List<string> {Error};
            else if (!Errors.Contains(Error))
                Errors.Add(Error);
        }

        public void SetFail(Exception e)
        {
            SetFail(e.Message);
            if (e.InnerException != null)
                SetFail(e.InnerException);
        }

        public void MergeResults(Result result, bool clearOnOk = false)
        {
            if (result == null)
                return;
            OK = OK && result.OK;
            if (!string.IsNullOrEmpty(result.Id))
                Id = result.Id;
            if (Errors == null)
                Errors = new List<string>();
            if (!OK)
            {
                if (result.Errors != null)
                    Errors.AddRange(result.Errors);
            }
            else if (clearOnOk)
                Errors.Clear();
        }

        public override string ToString()
        {
            return OK ? "OK" : string.Join("\n", Errors);
        }

        public static Result FailedResult(string Error = null)
        {
            var result = new Result();
            result.SetFail(Error ?? "Failed");
            return result;
        }
    }

    /// <summary>
    /// Generic version of FailedResult Factory
    /// </summary>
    /// <typeparam name="T">Result derived type</typeparam>
    public class Result<T> where T : Result, new()
    {
        public static T FailedResult(string Error = null)
        {
            var result = new T();
            result.SetFail(Error ?? "Failed");
            return result;
        }
    }

}