using System;
using System.Linq;
using ACTransit.Contracts.Data.Common;
using ACTransit.Contracts.Data.Customer.Feedback;
using ACTransit.DataAccess.CustomerRelations;

namespace ACTransit.CusRel.Public.Domain.Repositories.CustomerRelations.DAL
{
    public class CusRelRepository : IDisposable
    {
        private readonly CusRelEntities context;

        public CusRelRepository(CusRelEntities context)
        {
            this.context = context;
        }

        // =============================================================

        #region Bookkeeping

        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing)
                context.Dispose();
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        // =============================================================

        #region Lookup Tables and Enums

        #endregion

        // =============================================================

        #region Common 

        #endregion

        // =============================================================

        #region Feedback

        public int MaxFeedbackId
        {
            get
            {
                var max = context.tblContacts.Max(c => c.FeedbackId);
                return max != null ? max.Value : -1;
            }
        }

        public MethodResult AddCustomerContact(Form form)
        {
            var maxFeedbackId = MaxFeedbackId;
            if (maxFeedbackId == -1) return new MethodResult { OK = false, Message = "MaxFeedbackId not available" };
            var feedbackId = maxFeedbackId + 1;  // TODO: not using ACID here, consider changing

            int result;
            var incidentForm = form as IncidentForm;
            var lostFoundForm = form as LostFoundForm;
            if (incidentForm != null)
            {
                incidentForm.Incident.DateTime = TimeZoneInfo.ConvertTimeFromUtc(incidentForm.Incident.DateTime, TimeZoneInfo.Local);
                result = context.addCustomerContact(feedbackId,
                    "WEB", "WEB", "N", "N", "WEB", null, "Normal",
                    incidentForm.Contact.FirstName ?? " ", incidentForm.Contact.LastName ?? " ",
                    incidentForm.Contact.Address, null,
                    incidentForm.Contact.City, incidentForm.Contact.State ?? "CA",
                    incidentForm.Contact.ZipCode, incidentForm.Contact.HomePhone, incidentForm.Contact.WorkPhone,
                    incidentForm.Contact.MobilePhone, incidentForm.Contact.EmailAddress,
                    "None",
                    incidentForm.Incident.DateTime, incidentForm.Incident.Vehicle, incidentForm.Incident.Line,
                    incidentForm.Incident.Destination, incidentForm.Incident.Location,
                    incidentForm.Employee.Badge, incidentForm.Employee.Description,
                    (incidentForm.RequestResponse ? "Y" : "N"),
                    " ",
                    form.GetType() == typeof(LostFoundForm) ? "37. Lost Property" : "00. Web-Unspecified",
                    "5. Unassigned", " ", null, "New", "New",
                    incidentForm.Comments, "WEB",
                    null, null,
                    form.File != null && form.File.Base64Data != null ? form.File.Base64Data : null,
                    form.File != null && form.File.Filename != null ? form.File.Filename : null,
                    form.File != null && form.File.Filesize != 0 ? form.File.Filesize : 0,
                    form.File != null && form.File.ContentType != null ? form.File.ContentType : null);
            }
            else if (lostFoundForm != null)
            {
                lostFoundForm.Incident.DateTime = TimeZoneInfo.ConvertTimeFromUtc(lostFoundForm.Incident.DateTime, TimeZoneInfo.Local);
                result = context.addCustomerContact(feedbackId,
                    "WEB", "WEB", "N", "N", "WEB", null, "Normal",
                    form.Contact.FirstName ?? " ", form.Contact.LastName ?? " ", form.Contact.Address, null,
                    form.Contact.City, form.Contact.State ?? "CA",
                    form.Contact.ZipCode, form.Contact.HomePhone, form.Contact.WorkPhone,
                    form.Contact.MobilePhone, form.Contact.EmailAddress,
                    "None",
                    lostFoundForm.Incident.DateTime, lostFoundForm.Incident.Vehicle, lostFoundForm.Incident.Line,
                    null, null,
                    null, null,
                    (form.RequestResponse ? "Y" : "N"),
                    " ",
                    "37. Lost Property",
                    "5. Unassigned", " ", null, "New", "New",
                    form.Comments, "WEB",
                    lostFoundForm.LostItem.Category, lostFoundForm.LostItem.Type,
                    form.File != null && form.File.Base64Data != null ? form.File.Base64Data : null,
                    form.File != null && form.File.Filename != null ? form.File.Filename : null,
                    form.File != null && form.File.Filesize != 0 ? form.File.Filesize : 0,
                    form.File != null && form.File.ContentType != null ? form.File.ContentType : null);
            }
            else 
                result = context.addCustomerContact(feedbackId,
                    "WEB", "WEB", "N", "N", "WEB", null, "Normal",
                    form.Contact.FirstName ?? " ", form.Contact.LastName ?? " ", form.Contact.Address, null,
                    form.Contact.City, form.Contact.State ?? "CA",
                    form.Contact.ZipCode, form.Contact.HomePhone, form.Contact.WorkPhone,
                    form.Contact.MobilePhone, form.Contact.EmailAddress,
                    "None",
                    null, null, null,
                    null, null,
                    null, null,
                    (form.RequestResponse ? "Y" : "N"),
                    " ",
                    "00. Web-Unspecified",
                    "5. Unassigned", " ", null, "New", "New",
                    form.Comments, "WEB",
                    null, null,
                    form.File != null && form.File.Base64Data != null ? form.File.Base64Data : null,
                    form.File != null && form.File.Filename != null ? form.File.Filename : null,
                    form.File != null && form.File.Filesize != 0 ? form.File.Filesize : 0,
                    form.File != null && form.File.ContentType != null ? form.File.ContentType : null);

            return new MethodResult
            {
                OK = result == 1, 
                ID = feedbackId
            };
        }

        #endregion

        // =============================================================
    }
}
