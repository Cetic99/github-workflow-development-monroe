import UserMainScreen from '@ui/screens/retail/user-main'
import IdleScreen from '@ui/screens/retail/idle'
import { Route, Routes } from 'react-router-dom'
import SearchScreen from '@ui/screens/retail/search'
import CategoryScreen from '@ui/screens/retail/category'
import CollectionScreen from '@ui/screens/retail/collection'
import ProductScreen from '@ui/screens/retail/products'
import CartScreen from '@ui/screens/retail/cart'
import CheckoutScreen from '@ui/screens/retail/checkout'
import CashPaymentScreen from '@ui/screens/retail/cash-payment'
import CouponPaymentScreen from '@ui/screens/retail/coupon-payment'
import CardPaymentScreen from '@ui/screens/retail/card-payment'
import LoadingScreen from '@ui/screens/retail/loading'
import ReceiptScreen from '@ui/screens/retail/receipt'

const RetailUserRouter = () => {
  return (
    <Routes>
      <Route path="retail/" element={<UserMainScreen />} />
      <Route path="retail/idle" element={<IdleScreen />} />

      <Route path="retail/search" element={<SearchScreen />} />
      <Route path="retail/categories/:title" element={<CategoryScreen />} />
      <Route path="retail/collections/:title" element={<CollectionScreen />} />
      <Route path="retail/products/:title" element={<ProductScreen />} />

      <Route path="retail/cart" element={<CartScreen />} />
      <Route path="retail/checkout" element={<CheckoutScreen />} />

      <Route path="retail/cash-payment" element={<CashPaymentScreen />} />
      <Route path="retail/coupon-payment" element={<CouponPaymentScreen />} />
      <Route path="retail/card-payment" element={<CardPaymentScreen />} />

      <Route path="retail/receipt" element={<ReceiptScreen />} />

      <Route path="retail/loading" element={<LoadingScreen />} />
      <Route path="*" element={<UserMainScreen />} />
    </Routes>
  )
}

export default RetailUserRouter
