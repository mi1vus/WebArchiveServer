<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PagesList.ascx.cs" Inherits="WebArchiveServer.Controls.PagesList" %>

<%= CreateHomeLinkHtml() %>

<% foreach (string pageUrl in GetPages()) {
       Response.Write(pageUrl);       
}%>