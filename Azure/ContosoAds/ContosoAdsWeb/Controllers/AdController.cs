namespace ContosoAdsWeb.Controllers
{
    using System;
    using System.Data.Entity;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using ContosoAdsCommon;
    using Microsoft.WindowsAzure.ServiceRuntime;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Microsoft.WindowsAzure.Storage.RetryPolicies;

    public class AdController : Controller
    {
        private static CloudBlobContainer imagesBlobContainer;
        private readonly ContosoAdsContext db = new ContosoAdsContext();
        private CloudQueue imagesQueue;

        public AdController()
        {
            this.InitializeStorage();
        }

        private void InitializeStorage()
        {
            // Open storage account using credentials from .cscfg file.
            var storageAccount =
                CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));

            // Get context object for working with blobs, and 
            // set a default retry policy appropriate for a web user interface.
            var blobClient = storageAccount.CreateCloudBlobClient();
            blobClient.RetryPolicy = new LinearRetry(TimeSpan.FromSeconds(3), 3);

            // Get a reference to the blob container.
            imagesBlobContainer = blobClient.GetContainerReference("images");

            // Get context object for working with queues, and 
            // set a default retry policy appropriate for a web user interface.
            var queueClient = storageAccount.CreateCloudQueueClient();
            queueClient.RetryPolicy = new LinearRetry(TimeSpan.FromSeconds(3), 3);

            // Get a reference to the queue.
            this.imagesQueue = queueClient.GetQueueReference("images");
        }

        // GET: Ad
        public async Task<ActionResult> Index(int? category)
        {
            // This code executes an unbounded query; don't do this in a production app,
            // it could return too many rows for the web app to handle. For an example
            // of paging code, see:
            // http://www.asp.net/mvc/tutorials/getting-started-with-ef-using-mvc/sorting-filtering-and-paging-with-the-entity-framework-in-an-asp-net-mvc-application
            var adsList = this.db.Ads.AsQueryable();
            if (category != null)
            {
                adsList = adsList.Where(a => a.Category == (Category)category);
            }
            return this.View(await adsList.ToListAsync());
        }

        // GET: Ad/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var ad = await this.db.Ads.FindAsync(id);
            if (ad == null)
            {
                return this.HttpNotFound();
            }
            return this.View(ad);
        }

        // GET: Ad/Create
        public ActionResult Create()
        {
            return this.View();
        }

        // POST: Ad/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(
            [Bind(Include = "Title,Price,Description,Category,Phone")] Ad ad,
            HttpPostedFileBase imageFile)
        {
            // A production app would implement more robust input validation.
            // For example, validate that the image file size is not too large.
            if (this.ModelState.IsValid)
            {
                CloudBlockBlob imageBlob = null;
                if (imageFile != null && imageFile.ContentLength != 0)
                {
                    imageBlob = await this.UploadAndSaveBlobAsync(imageFile);
                    ad.ImageURL = imageBlob.Uri.ToString();
                }

                ad.PostedDate = DateTime.Now;
                this.db.Ads.Add(ad);
                await this.db.SaveChangesAsync();
                Trace.TraceInformation("Created AdId {0} in database", ad.AdId);

                if (imageBlob != null)
                {
                    var queueMessage = new CloudQueueMessage(ad.AdId.ToString());
                    await this.imagesQueue.AddMessageAsync(queueMessage);
                    Trace.TraceInformation("Created queue message for AdId {0}", ad.AdId);
                }

                return this.RedirectToAction("Index");
            }

            return this.View(ad);
        }

        // GET: Ad/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var ad = await this.db.Ads.FindAsync(id);
            if (ad == null)
            {
                return this.HttpNotFound();
            }

            return this.View(ad);
        }

        // POST: Ad/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(
            [Bind(Include = "AdId,Title,Price,Description,ImageURL,ThumbnailURL,PostedDate,Category,Phone")] Ad ad,
            HttpPostedFileBase imageFile)
        {
            CloudBlockBlob imageBlob = null;
            if (this.ModelState.IsValid)
            {
                if (imageFile != null && imageFile.ContentLength != 0)
                {
                    // User is changing the image -- delete the existing
                    // image blobs and then upload and save a new one.
                    await this.DeleteAdBlobsAsync(ad);
                    imageBlob = await this.UploadAndSaveBlobAsync(imageFile);
                    ad.ImageURL = imageBlob.Uri.ToString();
                }
                this.db.Entry(ad).State = EntityState.Modified;
                await this.db.SaveChangesAsync();
                Trace.TraceInformation("Updated AdId {0} in database", ad.AdId);

                if (imageBlob != null)
                {
                    var queueMessage = new CloudQueueMessage(ad.AdId.ToString());
                    await this.imagesQueue.AddMessageAsync(queueMessage);
                    Trace.TraceInformation("Created queue message for AdId {0}", ad.AdId);
                }
                return this.RedirectToAction("Index");
            }
            return this.View(ad);
        }

        // GET: Ad/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var ad = await this.db.Ads.FindAsync(id);
            if (ad == null)
            {
                return this.HttpNotFound();
            }
            return this.View(ad);
        }

        // POST: Ad/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var ad = await this.db.Ads.FindAsync(id);

            await this.DeleteAdBlobsAsync(ad);

            this.db.Ads.Remove(ad);
            await this.db.SaveChangesAsync();
            Trace.TraceInformation("Deleted ad {0}", ad.AdId);
            return this.RedirectToAction("Index");
        }

        private async Task<CloudBlockBlob> UploadAndSaveBlobAsync(HttpPostedFileBase imageFile)
        {
            Trace.TraceInformation("Uploading image file {0}", imageFile.FileName);

            var blobName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
            // Retrieve reference to a blob. 
            var imageBlob = imagesBlobContainer.GetBlockBlobReference(blobName);
            // Create the blob by uploading a local file.
            using (var fileStream = imageFile.InputStream)
            {
                await imageBlob.UploadFromStreamAsync(fileStream);
            }

            Trace.TraceInformation("Uploaded image file to {0}", imageBlob.Uri);

            return imageBlob;
        }

        private async Task DeleteAdBlobsAsync(Ad ad)
        {
            if (!string.IsNullOrWhiteSpace(ad.ImageURL))
            {
                var blobUri = new Uri(ad.ImageURL);
                await DeleteAdBlobAsync(blobUri);
            }

            if (!string.IsNullOrWhiteSpace(ad.ThumbnailURL))
            {
                var blobUri = new Uri(ad.ThumbnailURL);
                await DeleteAdBlobAsync(blobUri);
            }
        }

        private static async Task DeleteAdBlobAsync(Uri blobUri)
        {
            var blobName = blobUri.Segments[blobUri.Segments.Length - 1];
            Trace.TraceInformation("Deleting image blob {0}", blobName);
            var blobToDelete = imagesBlobContainer.GetBlockBlobReference(blobName);
            await blobToDelete.DeleteAsync();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
