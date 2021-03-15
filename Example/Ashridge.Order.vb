Imports MedatechUK.Deserialiser
Imports System.ComponentModel.Composition

<Export(GetType(ILexor))>
<ExportMetadata("LexName", "Ashridge.Order")>
<ExportMetadata("LexVers", "1.1")>
<ExportMetadata("Parser", eParser.json)>
<ExportMetadata("SerialType", GetType(Ashridge.Order))>
<ExportMetadata("LoadType", "ORD")>
Public Class AshridgeLexor
    Inherits Lexor
    Implements ILexor

End Class

Namespace Ashridge

    Public Class Order

        Private _order_id As String
        Private _reference_number As String
        Private _created_at As String
        Private _state As String
        Private _status As String
        Private _ashridge_ship_method As String
        Private _delivery_at As String
        Private _currency As String
        Private _customer As customer
        Private _billingaddress As address
        Private _shippingaddress As address
        Private _payment_method As payment_method
        Private _shipping_method As shipping_method
        Private _instruction_for_courier As String
        Private _message_for_ashridge As String
        Private _order_special_note As String
        Private _warehouse_special_note As String
        Private _orderitems As New List(Of orderitem)
        Private _subtotal As String
        Private _shipping_amount As String
        Private _discount_amount As String
        Private _discount_code As String
        Private _tax_amount As String
        Private _grandtotal As String
        Private _total_paid As String
        Private _total_refunded As String
        Private _total_due As String

        Public Property order_id As String
            Get
                Return _order_id
            End Get
            Set(value As String)
                _order_id = value
            End Set
        End Property
        Public Property reference_number As String
            Get
                Return _reference_number
            End Get
            Set(value As String)
                _reference_number = value
            End Set
        End Property
        Public Property created_at As String
            Get
                Return _created_at
            End Get
            Set(value As String)
                _created_at = value
            End Set
        End Property
        Public Property state As String
            Get
                Return _state
            End Get
            Set(value As String)
                _state = value
            End Set
        End Property
        Public Property status As String
            Get
                Return _status
            End Get
            Set(value As String)
                _status = value
            End Set
        End Property
        Public Property ashridge_ship_method As String
            Get
                Return _ashridge_ship_method
            End Get
            Set(value As String)
                _ashridge_ship_method = value
            End Set
        End Property
        Public Property delivery_at As String
            Get
                Return _delivery_at
            End Get
            Set(value As String)
                _delivery_at = value
            End Set
        End Property
        Public Property currency As String
            Get
                Return _currency
            End Get
            Set(value As String)
                _currency = value
            End Set
        End Property
        Public Property customer As customer
            Get
                Return _customer
            End Get
            Set(value As customer)
                _customer = value
            End Set
        End Property
        Public Property billingaddress As address
            Get
                Return _billingaddress
            End Get
            Set(value As address)
                _billingaddress = value
            End Set
        End Property
        Public Property shippingaddress As address
            Get
                Return _shippingaddress
            End Get
            Set(value As address)
                _shippingaddress = value
            End Set
        End Property
        Public Property payment_method As payment_method
            Get
                Return _payment_method
            End Get
            Set(value As payment_method)
                _payment_method = value
            End Set
        End Property
        Public Property shipping_method As shipping_method
            Get
                Return _shipping_method
            End Get
            Set(value As shipping_method)
                _shipping_method = value
            End Set
        End Property
        Public Property instruction_for_courier As String
            Get
                Return _instruction_for_courier
            End Get
            Set(value As String)
                _instruction_for_courier = value
            End Set
        End Property
        Public Property message_for_ashridge As String
            Get
                Return _message_for_ashridge
            End Get
            Set(value As String)
                _message_for_ashridge = value
            End Set
        End Property
        Public Property order_special_note As String
            Get
                Return _order_special_note
            End Get
            Set(value As String)
                _order_special_note = value
            End Set
        End Property
        Public Property warehouse_special_note As String
            Get
                Return _warehouse_special_note
            End Get
            Set(value As String)
                _warehouse_special_note = value
            End Set
        End Property
        Public Property orderitems As List(Of orderitem)
            Get
                Return _orderitems
            End Get
            Set(value As List(Of orderitem))
                _orderitems = value
            End Set
        End Property
        Public Property subtotal As String
            Get
                Return _subtotal
            End Get
            Set(value As String)
                _subtotal = value
            End Set
        End Property
        Public Property shipping_amount As String
            Get
                Return _shipping_amount
            End Get
            Set(value As String)
                _shipping_amount = value
            End Set
        End Property
        Public Property discount_amount As String
            Get
                Return _discount_amount
            End Get
            Set(value As String)
                _discount_amount = value
            End Set
        End Property
        Public Property discount_code As String
            Get
                Return _discount_code
            End Get
            Set(value As String)
                _discount_code = value
            End Set
        End Property
        Public Property tax_amount As String
            Get
                Return _tax_amount
            End Get
            Set(value As String)
                _tax_amount = value
            End Set
        End Property
        Public Property grandtotal As String
            Get
                Return _grandtotal
            End Get
            Set(value As String)
                _grandtotal = value
            End Set
        End Property
        Public Property total_paid As String
            Get
                Return _total_paid
            End Get
            Set(value As String)
                _total_paid = value
            End Set
        End Property
        Public Property total_refunded As String
            Get
                Return _total_refunded
            End Get
            Set(value As String)
                _total_refunded = value
            End Set
        End Property
        Public Property total_due As String
            Get
                Return _total_due
            End Get
            Set(value As String)
                _total_due = value
            End Set
        End Property
    End Class

    Public Class customer

        Private _id As String
        Private _prefix As String
        Private _firstName As String
        Private _lastName As String
        Private _email As String
        Private _group_id As String

        Public Property id As String
            Get
                Return _id
            End Get
            Set(value As String)
                _id = value
            End Set
        End Property
        Public Property prefix As String
            Get
                Return _prefix
            End Get
            Set(value As String)
                _prefix = value
            End Set
        End Property
        Public Property firstName As String
            Get
                Return _firstName
            End Get
            Set(value As String)
                _firstName = value
            End Set
        End Property
        Public Property lastName As String
            Get
                Return _lastName
            End Get
            Set(value As String)
                _lastName = value
            End Set
        End Property
        Public Property email As String
            Get
                Return _email
            End Get
            Set(value As String)
                _email = value
            End Set
        End Property
        Public Property group_id As String
            Get
                Return _group_id
            End Get
            Set(value As String)
                _group_id = value
            End Set
        End Property
    End Class

    Public Class address

        Private _prefix As String
        Private _firstName As String
        Private _lastName As String
        Private _company As String
        Private _street1 As String
        Private _street2 As String
        Private _city As String
        Private _state As String
        Private _postCode As String
        Private _country As String
        Private _telephone As String

        Public Property prefix As String
            Get
                Return _prefix
            End Get
            Set(value As String)
                _prefix = value
            End Set
        End Property
        Public Property firstName As String
            Get
                Return _firstName
            End Get
            Set(value As String)
                _firstName = value
            End Set
        End Property
        Public Property lastName As String
            Get
                Return _lastName
            End Get
            Set(value As String)
                _lastName = value
            End Set
        End Property
        Public Property company As String
            Get
                Return _company
            End Get
            Set(value As String)
                _company = value
            End Set
        End Property
        Public Property street1 As String
            Get
                Return _street1
            End Get
            Set(value As String)
                _street1 = value
            End Set
        End Property
        Public Property street2 As String
            Get
                Return _street2
            End Get
            Set(value As String)
                _street2 = value
            End Set
        End Property
        Public Property city As String
            Get
                Return _city
            End Get
            Set(value As String)
                _city = value
            End Set
        End Property
        Public Property state As String
            Get
                Return _state
            End Get
            Set(value As String)
                _state = value
            End Set
        End Property
        Public Property postCode As String
            Get
                Return _postCode
            End Get
            Set(value As String)
                _postCode = value
            End Set
        End Property
        Public Property country As String
            Get
                Return _country
            End Get
            Set(value As String)
                _country = value
            End Set
        End Property
        Public Property telephone As String
            Get
                Return _telephone
            End Get
            Set(value As String)
                _telephone = value
            End Set
        End Property
    End Class

    Public Class orderitem

        Private _name As String
        Private _sku As String
        Private _size As String
        Private _price As String
        Private _qty As String
        Private _subtotal As String
        Private _tax_percent As String
        Private _tax As String
        Private _discount_percent As String
        Private _discount As String
        Private _row_total As String

        Public Property name As String
            Get
                Return _name
            End Get
            Set(value As String)
                _name = value
            End Set
        End Property
        Public Property sku As String
            Get
                Return _sku
            End Get
            Set(value As String)
                _sku = value
            End Set
        End Property
        Public Property size As String
            Get
                Return _size
            End Get
            Set(value As String)
                _size = value
            End Set
        End Property
        Public Property price As String
            Get
                Return _price
            End Get
            Set(value As String)
                _price = value
            End Set
        End Property
        Public Property qty As String
            Get
                Return _qty
            End Get
            Set(value As String)
                _qty = value
            End Set
        End Property
        Public Property subtotal As String
            Get
                Return _subtotal
            End Get
            Set(value As String)
                _subtotal = value
            End Set
        End Property
        Public Property tax_percent As String
            Get
                Return _tax_percent
            End Get
            Set(value As String)
                _tax_percent = value
            End Set
        End Property
        Public Property tax As String
            Get
                Return _tax
            End Get
            Set(value As String)
                _tax = value
            End Set
        End Property
        Public Property discount_percent As String
            Get
                Return _discount_percent
            End Get
            Set(value As String)
                _discount_percent = value
            End Set
        End Property
        Public Property discount As String
            Get
                Return _discount
            End Get
            Set(value As String)
                _discount = value
            End Set
        End Property
        Public Property row_total As String
            Get
                Return _row_total
            End Get
            Set(value As String)
                _row_total = value
            End Set
        End Property
    End Class

    Public Class shipping_method

        Private _title As String
        Private _amount As String
        Private _currency As String

        Public Property title As String
            Get
                Return _title
            End Get
            Set(value As String)
                _title = value
            End Set
        End Property
        Public Property amount As String
            Get
                Return _amount
            End Get
            Set(value As String)
                _amount = value
            End Set
        End Property
        Public Property currency As String
            Get
                Return _currency
            End Get
            Set(value As String)
                _currency = value
            End Set
        End Property

    End Class

    Public Class payment_method

        Private _title As String
        Private _card_token As String

        Public Property title As String
            Get
                Return _title
            End Get
            Set(value As String)
                _title = value
            End Set
        End Property
        Public Property card_token As String
            Get
                Return _card_token
            End Get
            Set(value As String)
                _card_token = value
            End Set
        End Property
    End Class

End Namespace