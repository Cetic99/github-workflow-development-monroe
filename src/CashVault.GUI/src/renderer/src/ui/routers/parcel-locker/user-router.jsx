/* eslint-disable prettier/prettier */
import { Route, Routes } from 'react-router-dom'

import IdleScreen from '@screens/parcel-locker/idle'
import SelectLanguageScreen from '@screens/parcel-locker/select-language'
import SelectParcelActionScreen from '@screens/parcel-locker/select-parcel-action'
import SelectCourierScreen from '@screens/parcel-locker/select-courier'
import SelectPickupMethodScreen from '@screens/parcel-locker/select-pickup-method'
import PickupEnterCodeScreen from '@screens/parcel-locker/pickup-enter-code'
import PickupScanQrScreen from '@screens/parcel-locker/pickup-scan-qr'
import ProcessingScreen from '@screens/parcel-locker/processing'
import SendSelectPaymentOptionsScreen from '@screens/parcel-locker/send-select-payment-options'
import PickupSelectPaymentOptionsScreen from '@screens/parcel-locker/pickup-select-payment-options'
import CashPaymentScreen from '@screens/parcel-locker/cash-payment'
import CardPaymentScreen from '@screens/parcel-locker/card-payment'
import TakePackageScreen from '@screens/parcel-locker/take-package'
import SelectLockerSizeScreen from '@screens/parcel-locker/select-locker-size'
import SelectDeliveryOptionScreen from '@screens/parcel-locker/select-delivery-option'
import EnterParcelAddressScreen from '@screens/parcel-locker/enter-parcel-address'
import EnterRecipientDetailsScreen from '@screens/parcel-locker/enter-recipient-details'
import EnterSenderPhoneNumberScreen from '@screens/parcel-locker/enter-sender-phone-number'
import EnterRecipientPhoneNumberScreen from '@screens/parcel-locker/enter-recipient-phone-number'
import SendScanQrScreen from '@screens/parcel-locker/send-scan-qr'
import ConfirmDeliveryScreen from '@screens/parcel-locker/confirm-delivery'
import InsertPackageScreen from '@screens/parcel-locker/insert-package'
import TakeReceiptScreen from '@screens/parcel-locker/take-receipt'
import PackageSentScreen from '@screens/parcel-locker/package-sent'
import CouponPaymentScreen from '@screens/parcel-locker/coupon-payment'
import CheckCouponScreen from '@screens/parcel-locker/check-coupon'
import MapFiltersScreen from '@screens/parcel-locker/map-filters'
import ShipmentsScreen from '@ui/screens/parcel-locker/shipments'
import OperatorCodeScreen from '@ui/screens/parcel-locker/operator-code'
import AdminLoginScreen from '@ui/screens/parcel-locker/admin-login'
import ErrorScreen from '@ui/screens/parcel-locker/error'
import ScanParcelScreen from '@ui/screens/parcel-locker/scan-parcel'
import GetParcelScreen from '@ui/screens/parcel-locker/get-parcel'

const ParcelLockerUserRouter = () => {
  return (
    <Routes>
      <Route path="parcel-locker/idle" element={<IdleScreen />} />
      <Route path="parcel-locker/select-language" element={<SelectLanguageScreen />} />
      <Route path="parcel-locker/select-parcel-action" element={<SelectParcelActionScreen />} />
      <Route path="parcel-locker/select-courier/:method" element={<SelectCourierScreen />} />
      <Route path="parcel-locker/select-pickup-method" element={<SelectPickupMethodScreen />} />
      <Route path="parcel-locker/pickup-enter-code" element={<PickupEnterCodeScreen />} />
      <Route path="parcel-locker/pickup-scan-qr" element={<PickupScanQrScreen />} />
      <Route path="parcel-locker/processing/:state/:text" element={<ProcessingScreen />} />
      <Route
        path="parcel-locker/send-select-payment-options"
        element={<SendSelectPaymentOptionsScreen />}
      />
      <Route
        path="parcel-locker/pickup-payment-options"
        element={<PickupSelectPaymentOptionsScreen />}
      />
      <Route path="parcel-locker/cash-payment/:method" element={<CashPaymentScreen />} />
      <Route path="parcel-locker/card-payment/:method" element={<CardPaymentScreen />} />
      <Route path="parcel-locker/coupon-payment" element={<CouponPaymentScreen />} />
      <Route path="parcel-locker/check-coupon" element={<CheckCouponScreen />} />
      <Route path="parcel-locker/take-package" element={<TakePackageScreen />} />

      <Route path="parcel-locker/select-locker-size" element={<SelectLockerSizeScreen />} />
      <Route path="parcel-locker/select-delivery-option" element={<SelectDeliveryOptionScreen />} />
      <Route path="parcel-locker/enter-parcel-address" element={<EnterParcelAddressScreen />} />
      <Route
        path="parcel-locker/enter-recipient-details"
        element={<EnterRecipientDetailsScreen />}
      />
      <Route
        path="parcel-locker/enter-sender-phone-number"
        element={<EnterSenderPhoneNumberScreen />}
      />
      <Route
        path="parcel-locker/enter-recipient-phone-number"
        element={<EnterRecipientPhoneNumberScreen />}
      />
      <Route path="parcel-locker/send-scan-qr" element={<SendScanQrScreen />} />
      <Route path="parcel-locker/confirm-delivery" element={<ConfirmDeliveryScreen />} />
      <Route path="parcel-locker/insert-package" element={<InsertPackageScreen />} />
      <Route path="parcel-locker/take-receipt" element={<TakeReceiptScreen />} />
      <Route path="parcel-locker/package-sent" element={<PackageSentScreen />} />
      <Route path="parcel-locker/map-filters" element={<MapFiltersScreen />} />

      <Route path="parcel-locker/error" element={<ErrorScreen />} />

      <Route path="parcel-locker/operator-code" element={<OperatorCodeScreen />} />
      <Route path="parcel-locker/admin-login" element={<AdminLoginScreen />} />
      <Route path="parcel-locker/shipments" element={<ShipmentsScreen />} />
      <Route path="parcel-locker/scan-parcel/:method" element={<ScanParcelScreen />} />
      <Route path="parcel-locker/scan-code/:admin" element={<PickupScanQrScreen />} />
      <Route path="parcel-locker/enter-code/:admin" element={<PickupEnterCodeScreen />} />
      <Route path="parcel-locker/get-parcel" element={<GetParcelScreen />} />
      <Route
        path="parcel-locker/insert-parcel/:admin/:lockerId?"
        element={<InsertPackageScreen />}
      />
      <Route path="parcel-locker/take-parcel/:admin?/:lockerId?" element={<TakePackageScreen />} />

      <Route path="*" element={<SelectLanguageScreen />} />
    </Routes>
  )
}

export default ParcelLockerUserRouter
