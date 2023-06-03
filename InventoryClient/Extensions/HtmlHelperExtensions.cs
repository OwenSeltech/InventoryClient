using System;
using System.Linq.Expressions;
using System.Text;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;


namespace InventoryClient.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static IHtmlContent NumberTextBoxFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper,
          Expression<Func<TModel, TProperty>> expression, object htmlAttributes = null)
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException("Expression must be a member expression");

            var modelValue = htmlHelper.ViewData.Model != null ? expression.Compile()(htmlHelper.ViewData.Model) : default(TProperty);
            var valueString = modelValue != null ? modelValue.ToString() : string.Empty;
            if(valueString.ToString() ==  "0") valueString = string.Empty;

            var inputTag = new TagBuilder("input");
            inputTag.TagRenderMode = TagRenderMode.SelfClosing;
            inputTag.Attributes.Add("type", "number");
            inputTag.Attributes.Add("name", memberExpression.Member.Name);
            inputTag.Attributes.Add("value", valueString?.ToString());

            if (htmlAttributes != null)
            {
                var attributes = htmlAttributes.GetType().GetProperties();
                foreach (var attribute in attributes)
                {
                    inputTag.Attributes.Add(attribute.Name, attribute.GetValue(htmlAttributes)?.ToString());
                }
            }

            return inputTag;
        }
        public static IHtmlContent EmailInputFor<TModel, TResult>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TResult>> expression, string cssClass = null)
        {
            // Generate the email input element based on the expression
            var emailInput = htmlHelper.TextBoxFor(expression, new { type = "email", @class = cssClass });

            // Add custom email validation attributes
            emailInput = emailInput
                .Attr("pattern", @"^[\w\.-]+@[\w\.-]+\.\w+$")
                .Attr("title", "Please enter a valid email address");

            return emailInput;
        }

        private static IHtmlContent Attr(this IHtmlContent content, string attribute, string value)
        {
            var writer = new System.IO.StringWriter();
            content.WriteTo(writer, System.Text.Encodings.Web.HtmlEncoder.Default);
            var modifiedContent = writer.ToString().TrimEnd('>');
            modifiedContent += $" {attribute}=\"{value}\">";
            return new HtmlString(modifiedContent);
        }
    }
}
