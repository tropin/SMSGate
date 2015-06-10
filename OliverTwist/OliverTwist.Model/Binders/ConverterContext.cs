using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Csharper.OliverTwist.Model.Binders
{
    public class ConverterContext : ITypeDescriptorContext
    {
        private PropertyDescriptor _propertyDescriptor;
        
        #region ITypeDescriptorContext Members

        public IContainer Container
        {
            get { throw new NotImplementedException(); }
        }

        public object Instance
        {
            get { throw new NotImplementedException(); }
        }

        public void OnComponentChanged()
        {
            throw new NotImplementedException();
        }

        public bool OnComponentChanging()
        {
            throw new NotImplementedException();
        }

        public PropertyDescriptor PropertyDescriptor
        {
            get;
            private set;
        }

        #endregion

        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            throw new NotImplementedException();
        }

        #endregion

        public ConverterContext(PropertyDescriptor pdesc)
        {
            PropertyDescriptor = pdesc;
        }
    }
}
