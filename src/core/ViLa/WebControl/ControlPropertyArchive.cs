using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ViLa.Model;
using WebExpress.Attribute;
using WebExpress.Html;
using WebExpress.Internationalization;
using WebExpress.UI.Attribute;
using WebExpress.UI.Component;
using WebExpress.UI.WebControl;
using WebExpress.WebApp.Components;
using WebExpress.WebApp.WebControl;

namespace ViLa.WebControl
{
    [Section(Section.PropertyPrimary)]
    [Application("ViLa")]
    [Context("details")]
    public sealed class ControlPropertyArchive : ControlButtonLink, IComponent
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public ControlPropertyArchive()
            : base("archive_btn")
        {
            Init();
        }

        /// <summary>
        /// Initialisierung
        /// </summary>
        private void Init()
        {
            Margin = new PropertySpacingMargin(PropertySpacing.Space.Two);
            BackgroundColor = new PropertyColorButton(TypeColorButton.Primary);
            Icon = new PropertyIcon(TypeIcon.Clock);
            TextColor = new PropertyColorText(TypeColorText.Light);
        }

        /// <summary>
        /// In HTML konvertieren
        /// </summary>
        /// <param name="context">Der Kontext, indem das Steuerelement dargestellt wird</param>
        /// <returns>Das Control als HTML</returns>
        public override IHtmlNode Render(RenderContext context)
        {
            Text = context.I18N("vila.archive.label");

            var modal = new ControlModalFormConfirm()
            {
                Header = context.Page.I18N("vila.archive.label"),
                Content = new ControlFormularItemStaticText() { Text = context.I18N("vila.archive.description") },
                RedirectUri = context.Uri.Take(-1)
            };

            modal.Confirm += (s, e) =>
            {
                var id = context.Page.GetParamValue("id");
                ViewModel.Instance.ArchiveHistoryMeasurementLog(id);

                context.Page.Redirecting(context.Uri.Take(-1));
            };

            Modal = modal;

            return base.Render(context);
        }
    }
}
