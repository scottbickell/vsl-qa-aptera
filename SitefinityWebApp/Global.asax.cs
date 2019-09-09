using SitefinityWebApp.Services;
using System;
using System.Linq;
using System.Web;
using Telerik.Microsoft.Practices.Unity;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.SitemapGenerator.Abstractions.Events;

namespace SitefinityWebApp {
    public class Global : HttpApplication {

        protected void Application_Start(object sender, EventArgs e) {
            Bootstrapper.Initialized += Bootstrapper_Initialized;
        }

        protected void Bootstrapper_Initialized(object sender, Telerik.Sitefinity.Data.ExecutedEventArgs args) {
            if (args.CommandName == "Bootstrapped") {
                ObjectFactory.Container.RegisterType<Telerik.Sitefinity.Services.Search.Data.LuceneSearchService, CustomLuceneSearchService>(new ContainerControlledLifetimeManager());
                EventHub.Subscribe<ISitemapGeneratorBeforeWriting>(SitemapGeneratorBeforeWriting);
            }
        }

        private void SitemapGeneratorBeforeWriting(ISitemapGeneratorBeforeWriting @event) {
            if (Config.Get<SystemConfig>().SslOffloadingSettings.EnableSslOffloading) {
                var entries = @event.Entries;
                foreach (var entry in entries) {
                    entry.Location = entry.Location.Replace("http://", "https://");
                    if (entry.Rels != null) {
                        for (int i = 0; i < entry.Rels.Count; i++) {
                            var rel = entry.Rels.ElementAt(i);
                            if (rel.Value != null) {
                                string newValue = rel.Value.Replace("http://", "https://");
                                entry.Rels[rel.Key] = newValue;
                            }
                        }
                    }
                }

                @event.Entries = entries;
            }
        }

        //protected void Session_Start(object sender, EventArgs e) {

        //}

        protected void Application_BeginRequest(object sender, EventArgs e) {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e) {

        }

        protected void Application_Error(object sender, EventArgs e) {

        }

        //protected void Session_End(object sender, EventArgs e) {

        //}

        protected void Application_End(object sender, EventArgs e) {

        }
    }
}