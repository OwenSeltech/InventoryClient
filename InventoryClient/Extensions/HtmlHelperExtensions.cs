using System;
using System.Linq.Expressions;
using System.Text;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

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
    }
}
