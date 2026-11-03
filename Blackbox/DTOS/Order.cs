namespace Laboration.DTOS;
// NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
public partial class Order
{

    private string orderIdField = null!;

    private System.DateTime orderDateField;

    private OrderCustomer customerField = null!;

    private OrderBillingAddress billingAddressField = null!;

    private OrderShippingAddress? shippingAddressField;

    private OrderOrderLine[] itemsField = null!;

    private decimal totalAmountField;

    private string currencyField = null!;

    private uint routingNumberField;

    /// <remarks/>
    public string OrderId
    {
        get
        {
            return this.orderIdField;
        }
        set
        {
            this.orderIdField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
    public System.DateTime OrderDate
    {
        get
        {
            return this.orderDateField;
        }
        set
        {
            this.orderDateField = value;
        }
    }

    /// <remarks/>
    public OrderCustomer Customer
    {
        get
        {
            return this.customerField;
        }
        set
        {
            this.customerField = value;
        }
    }

    /// <remarks/>
    public OrderBillingAddress BillingAddress
    {
        get
        {
            return this.billingAddressField;
        }
        set
        {
            this.billingAddressField = value;
        }
    }

    /// <remarks/>
    public OrderShippingAddress? ShippingAddress
    {
        get
        {
            return this.shippingAddressField;
        }
        set
        {
            this.shippingAddressField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("OrderLine", IsNullable = false)]
    public OrderOrderLine[] Items
    {
        get
        {
            return this.itemsField;
        }
        set
        {
            this.itemsField = value;
        }
    }

    /// <remarks/>
    public decimal TotalAmount
    {
        get
        {
            return this.totalAmountField;
        }
        set
        {
            this.totalAmountField = value;
        }
    }

    /// <remarks/>
    public string Currency
    {
        get
        {
            return this.currencyField;
        }
        set
        {
            this.currencyField = value;
        }
    }

    /// <remarks/>
    public uint RoutingNumber
    {
        get
        {
            return this.routingNumberField;
        }
        set
        {
            this.routingNumberField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class OrderCustomer
{

    private string customerIdField = null!;

    private string companyNameField = null!;

    private OrderCustomerContactPerson contactPersonField = null!;

    private OrderCustomerSecondaryContactPerson? secondaryContactPersonField;

    /// <remarks/>
    public string CustomerId
    {
        get
        {
            return this.customerIdField;
        }
        set
        {
            this.customerIdField = value;
        }
    }

    /// <remarks/>
    public string CompanyName
    {
        get
        {
            return this.companyNameField;
        }
        set
        {
            this.companyNameField = value;
        }
    }

    /// <remarks/>
    public OrderCustomerContactPerson ContactPerson
    {
        get
        {
            return this.contactPersonField;
        }
        set
        {
            this.contactPersonField = value;
        }
    }

    /// <remarks/>
    public OrderCustomerSecondaryContactPerson? SecondaryContactPerson
    {
        get
        {
            return this.secondaryContactPersonField;
        }
        set
        {
            this.secondaryContactPersonField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class OrderCustomerContactPerson
{

    private string nameField = null!;

    private string? phoneField;

    /// <remarks/>
    public string Name
    {
        get
        {
            return this.nameField;
        }
        set
        {
            this.nameField = value;
        }
    }

    /// <remarks/>
    public string? Phone
    {
        get
        {
            return this.phoneField;
        }
        set
        {
            this.phoneField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class OrderCustomerSecondaryContactPerson
{

    private string nameField = null!;

    private string? emailField;

    private string? phoneField;

    /// <remarks/>
    public string Name
    {
        get
        {
            return this.nameField;
        }
        set
        {
            this.nameField = value;
        }
    }

    /// <remarks/>
    public string? Email
    {
        get
        {
            return this.emailField;
        }
        set
        {
            this.emailField = value;
        }
    }

    /// <remarks/>
    public string? Phone
    {
        get
        {
            return this.phoneField;
        }
        set
        {
            this.phoneField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class OrderBillingAddress
{

    private string streetField = null!;

    private string postalCodeField = null!;

    private string cityField = null!;

    private string countryField = null!;

    /// <remarks/>
    public string Street
    {
        get
        {
            return this.streetField;
        }
        set
        {
            this.streetField = value;
        }
    }

    /// <remarks/>
    public string PostalCode
    {
        get
        {
            return this.postalCodeField;
        }
        set
        {
            this.postalCodeField = value;
        }
    }

    /// <remarks/>
    public string City
    {
        get
        {
            return this.cityField;
        }
        set
        {
            this.cityField = value;
        }
    }

    /// <remarks/>
    public string Country
    {
        get
        {
            return this.countryField;
        }
        set
        {
            this.countryField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class OrderShippingAddress
{

    private string streetField = null!;

    private string postalCodeField = null!;

    private string cityField = null!;

    private string countryField = null!;

    /// <remarks/>
    public string Street
    {
        get
        {
            return this.streetField;
        }
        set
        {
            this.streetField = value;
        }
    }

    /// <remarks/>
    public string PostalCode
    {
        get
        {
            return this.postalCodeField;
        }
        set
        {
            this.postalCodeField = value;
        }
    }

    /// <remarks/>
    public string City
    {
        get
        {
            return this.cityField;
        }
        set
        {
            this.cityField = value;
        }
    }

    /// <remarks/>
    public string Country
    {
        get
        {
            return this.countryField;
        }
        set
        {
            this.countryField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class OrderOrderLine
{

    private object[] itemsField = null!;

    private ItemsChoiceType[] itemsElementNameField = null!;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("CustomDescription", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("Discount", typeof(OrderOrderLineDiscount))]
    [System.Xml.Serialization.XmlElementAttribute("Material", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("Packaging", typeof(OrderOrderLinePackaging))]
    [System.Xml.Serialization.XmlElementAttribute("Product", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("Quantity", typeof(ushort))]
    [System.Xml.Serialization.XmlElementAttribute("UnitPrice", typeof(decimal))]
    [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemsElementName")]
    public object[] Items
    {
        get
        {
            return this.itemsField;
        }
        set
        {
            this.itemsField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("ItemsElementName")]
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public ItemsChoiceType[] ItemsElementName
    {
        get
        {
            return this.itemsElementNameField;
        }
        set
        {
            this.itemsElementNameField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class OrderOrderLineDiscount
{

    private decimal fixedAmountField;

    private bool fixedAmountFieldSpecified;

    private string? codeField;

    private byte percentageField;

    private bool percentageFieldSpecified;

    /// <remarks/>
    public decimal FixedAmount
    {
        get
        {
            return this.fixedAmountField;
        }
        set
        {
            this.fixedAmountField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool FixedAmountSpecified
    {
        get
        {
            return this.fixedAmountFieldSpecified;
        }
        set
        {
            this.fixedAmountFieldSpecified = value;
        }
    }

    /// <remarks/>
    public string? Code
    {
        get
        {
            return this.codeField;
        }
        set
        {
            this.codeField = value;
        }
    }

    /// <remarks/>
    public byte Percentage
    {
        get
        {
            return this.percentageField;
        }
        set
        {
            this.percentageField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool PercentageSpecified
    {
        get
        {
            return this.percentageFieldSpecified;
        }
        set
        {
            this.percentageFieldSpecified = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class OrderOrderLinePackaging
{

    private decimal weightKgField;

    private OrderOrderLinePackagingPackageDimensions? packageDimensionsField;

    /// <remarks/>
    public decimal WeightKg
    {
        get
        {
            return this.weightKgField;
        }
        set
        {
            this.weightKgField = value;
        }
    }

    /// <remarks/>
    public OrderOrderLinePackagingPackageDimensions? PackageDimensions
    {
        get
        {
            return this.packageDimensionsField;
        }
        set
        {
            this.packageDimensionsField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class OrderOrderLinePackagingPackageDimensions
{

    private decimal lengthCmField;

    private decimal widthCmField;

    private decimal heightCmField;

    /// <remarks/>
    public decimal LengthCm
    {
        get
        {
            return this.lengthCmField;
        }
        set
        {
            this.lengthCmField = value;
        }
    }

    /// <remarks/>
    public decimal WidthCm
    {
        get
        {
            return this.widthCmField;
        }
        set
        {
            this.widthCmField = value;
        }
    }

    /// <remarks/>
    public decimal HeightCm
    {
        get
        {
            return this.heightCmField;
        }
        set
        {
            this.heightCmField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(IncludeInSchema = false)]
public enum ItemsChoiceType
{

    /// <remarks/>
    CustomDescription,

    /// <remarks/>
    Discount,

    /// <remarks/>
    Material,

    /// <remarks/>
    Packaging,

    /// <remarks/>
    Product,

    /// <remarks/>
    Quantity,

    /// <remarks/>
    UnitPrice,
}


