﻿using System;
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
        }

        /// <summary>
        /// In HTML konvertieren
        /// </summary>
        /// <param name="context">Der Kontext, indem das Steuerelement dargestellt wird</param>
        /// <returns>Das Control als HTML</returns>
        public override IHtmlNode Render(RenderContext context)
        {
            var form = new ControlFormular("arcive") { EnableSubmitAndNextButton = false, EnableCancelButton = false, RedirectUri = Uri };
            form.SubmitButton.Text = context.Page.I18N("vila.archive.label");
            form.SubmitButton.Icon = new PropertyIcon(TypeIcon.Clock);
            form.SubmitButton.Color = new PropertyColorButton(TypeColorButton.Primary);
            form.ProcessFormular += (s, e) =>
            {
                var id = context.Page.GetParamValue("id");
                ViewModel.Instance.ArchiveHistoryMeasurementLog(id);

                context.Page.Redirecting(context.Uri.Take(-1));
            };

            Text = context.I18N("vila.archive.label");
            BackgroundColor = new PropertyColorButton(TypeColorButton.Primary);
            Icon = new PropertyIcon(TypeIcon.Clock);
            TextColor = new PropertyColorText(TypeColorText.Light);
            Modal = new ControlModal
            (
                "archive",
                context.Page.I18N("vila.archive.label"),
                new ControlText()
                {
                    Text = context.Page.I18N("vila.archive.description")
                },
                form
            );

            return base.Render(context);
        }
    }
}
