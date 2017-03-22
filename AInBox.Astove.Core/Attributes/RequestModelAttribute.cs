using System;

namespace AInBox.Astove.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class RequestModelAttribute : Attribute
    {
        public Type PostRequestModel { get; set; }
        public Type PutRequestModel { get; set; }
        
        public RequestModelAttribute() { }
        public RequestModelAttribute(Type postRequestModel, Type putRequestModel) 
        { 
            this.PostRequestModel = postRequestModel;
            this.PutRequestModel = putRequestModel;
        }
    }
}