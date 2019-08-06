﻿using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using CefSharp;
using CefSharp.Handler;
using web2win.Plugins;

namespace web2win
{
    class WebViewHandlers : ISchemeHandlerFactory, IContextMenuHandler, ILifeSpanHandler, IRequestHandler
    {
        public IResourceHandler Create(IBrowser browser, IFrame frame, string schemeName, IRequest request)
            => new ResourceHandler();

        public void OnBeforeContextMenu(IWebBrowser chromiumWebBrowser, IBrowser browser,
            IFrame frame, IContextMenuParams parameters, IMenuModel model)
            => PlugInManager.Execute(new PluginEventArgs(new
            {
                chromiumWebBrowser,
                browser,
                frame,
                parameters,
                model
            }));

        public bool OnContextMenuCommand(IWebBrowser chromiumWebBrowser, IBrowser browser,
            IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
            => false;
        public void OnContextMenuDismissed(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame) { }

        public bool RunContextMenu(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame,
            IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback)
            => false;


        public bool OnBeforePopup(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame,
            string targetUrl, string targetFrameName, WindowOpenDisposition targetDisposition, bool userGesture,
            IPopupFeatures popupFeatures, IWindowInfo windowInfo, IBrowserSettings browserSettings,
            ref bool noJavascriptAccess, out IWebBrowser newBrowser)
        {
            var args = new PluginEventArgs(new
            {
                chromiumWebBrowser,
                browser,
                frame,
                targetUrl,
                targetFrameName,
                targetDisposition,
                userGesture,
                popupFeatures,
                windowInfo,
                browserSettings,
                noJavascriptAccess
            });

            if (args.Execute())
            {
                noJavascriptAccess = args.Get<bool>("noJavascriptAccess");
                newBrowser = args.Get<IWebBrowser>("newBrowser");
                return args.GetResult(true);
            }
            else
            {
                newBrowser = null;
                return true;
            }
        }
        public void OnAfterCreated(IWebBrowser chromiumWebBrowser, IBrowser browser)
            => PlugInManager.Execute(new PluginEventArgs(new { chromiumWebBrowser, browser }));
        public bool DoClose(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
            var args = new PluginEventArgs(new { chromiumWebBrowser, browser });
            args.Execute();
            return args.GetResult(false);
        }
        public void OnBeforeClose(IWebBrowser chromiumWebBrowser, IBrowser browser)
            => PlugInManager.Execute(new PluginEventArgs(new { chromiumWebBrowser, browser }));

        IRequestHandler request = new DefaultRequestHandler();

        public bool OnBeforeBrowse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool userGesture, bool isRedirect)
        {

            var args = new PluginEventArgs(new { chromiumWebBrowser, browser, frame, request, userGesture, isRedirect });
            args.Execute();
            return args.GetResult(false);
        }

        public bool OnOpenUrlFromTab(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, string targetUrl, 
            WindowOpenDisposition targetDisposition, bool userGesture) 
            => request.OnOpenUrlFromTab(chromiumWebBrowser, browser, frame, targetUrl, targetDisposition, userGesture);

        public bool OnCertificateError(IWebBrowser chromiumWebBrowser, IBrowser browser, CefErrorCode errorCode, string requestUrl, ISslInfo sslInfo, IRequestCallback callback) => request.OnCertificateError(chromiumWebBrowser, browser, errorCode, requestUrl, sslInfo, callback);
        public void OnPluginCrashed(IWebBrowser chromiumWebBrowser, IBrowser browser, string pluginPath) => request.OnPluginCrashed(chromiumWebBrowser, browser, pluginPath);
        public CefReturnValue OnBeforeResourceLoad(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback) => this.request.OnBeforeResourceLoad(chromiumWebBrowser, browser, frame, request, callback);
        public bool GetAuthCredentials(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, bool isProxy, string host, int port, string realm, string scheme, IAuthCallback callback) => request.GetAuthCredentials(chromiumWebBrowser, browser, frame, isProxy, host, port, realm, scheme, callback);
        public bool OnSelectClientCertificate(IWebBrowser chromiumWebBrowser, IBrowser browser, bool isProxy, string host, int port, X509Certificate2Collection certificates, ISelectClientCertificateCallback callback) => request.OnSelectClientCertificate(chromiumWebBrowser, browser, isProxy, host, port, certificates, callback);
        public void OnRenderProcessTerminated(IWebBrowser chromiumWebBrowser, IBrowser browser, CefTerminationStatus status) => request.OnRenderProcessTerminated(chromiumWebBrowser, browser, status);
        public bool CanGetCookies(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request) => this.request.CanGetCookies(chromiumWebBrowser, browser, frame, request);
        public bool CanSetCookie(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, Cookie cookie) => this.request.CanSetCookie(chromiumWebBrowser, browser, frame, request, cookie);
        public bool OnQuotaRequest(IWebBrowser chromiumWebBrowser, IBrowser browser, string originUrl, long newSize, IRequestCallback callback) => request.OnQuotaRequest(chromiumWebBrowser, browser, originUrl, newSize, callback);
        public void OnResourceRedirect(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response, ref string newUrl) => this.request.OnResourceRedirect(chromiumWebBrowser, browser, frame, request, response, ref newUrl);
        public bool OnProtocolExecution(IWebBrowser chromiumWebBrowser, IBrowser browser, string url) => request.OnProtocolExecution(chromiumWebBrowser, browser, url);
        public void OnRenderViewReady(IWebBrowser chromiumWebBrowser, IBrowser browser) => request.OnRenderViewReady(chromiumWebBrowser, browser);
        public bool OnResourceResponse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response) => this.request.OnResourceResponse(chromiumWebBrowser, browser, frame, request, response);
        public IResponseFilter GetResourceResponseFilter(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response) => this.request.GetResourceResponseFilter(chromiumWebBrowser, browser, frame, request, response);
        public void OnResourceLoadComplete(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request,
            IResponse response, UrlRequestStatus status, long receivedContentLength)
            => this.request.OnResourceLoadComplete(chromiumWebBrowser, browser, frame, request, response, status, receivedContentLength);
    }
}
