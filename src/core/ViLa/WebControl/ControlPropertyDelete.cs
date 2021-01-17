using System;
using System.IO;
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
    [Section(Section.PropertySecondary)]
    [Application("ViLa")]
    [Context("details")]
    public sealed class ControlPropertyDelete : ControlButtonLink, IComponent
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public ControlPropertyDelete()
            : base("delete_btn")
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
            var form = new ControlFormular("del") { EnableSubmitAndNextButton = false, EnableCancelButton = false, RedirectUri = Uri };
            form.SubmitButton.Text = context.Page.I18N("vila.delete.label");
            form.SubmitButton.Icon = new PropertyIcon(TypeIcon.TrashAlt);
            form.SubmitButton.Color = new PropertyColorButton(TypeColorButton.Danger);
            form.ProcessFormular += (s, e) =>
            {
                var id = context.Page.GetParamValue("id");
                try
                {
                    File.Delete(System.IO.Path.Combine(ViewModel.Instance.Context.Host.AssetPath, "measurements", id + ".xml"));
                    ViewModel.Instance.Logging.Add(new LogItem(LogItem.LogLevel.Info, string.Format("Datei {0}.xml wurde gelöscht!", id)));
                }
                catch (Exception ex)
                {
                    ViewModel.Instance.Logging.Add(new LogItem(LogItem.LogLevel.Exception, ex.ToString()));
                }

                context.Page.Redirecting(context.Uri.Take(-1));
            };

            Text = context.I18N("vila.delete.label");
            BackgroundColor = new PropertyColorButton(TypeColorButton.Danger);
            Icon = new PropertyIcon(TypeIcon.Trash);
            TextColor = new PropertyColorText(TypeColorText.Light);
            Modal = new ControlModal
            (
                "delete",
                context.Page.I18N("vila.delete.label"),
                new ControlText()
                {
                    Text = context.Page.I18N("vila.delete.description")
                },
                form
            );

            return base.Render(context);
        }
    }
}
