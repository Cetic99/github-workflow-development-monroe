/* eslint-disable prettier/prettier */
import { Route, Routes } from 'react-router-dom'

import ShipmentsScreen from '@ui/screens/parcel-locker/shipments'
import ScanParcelScreen from '@ui/screens/parcel-locker/scan-parcel'
import InsertPackageScreen from '@ui/screens/parcel-locker/insert-package'
import TakePackageScreen from '@ui/screens/parcel-locker/take-package'
import PickupScanQrScreen from '@ui/screens/parcel-locker/pickup-scan-qr'

const OperatorRouter = () => {
  return (
    <Routes>
      <Route path="/" element={<ShipmentsScreen />} />

      <Route path="/scan-parcel" element={<ScanParcelScreen />} />
      <Route path="/scan-code/:admin" element={<PickupScanQrScreen />} />
      <Route path="/insert-parcel/:admin" element={<InsertPackageScreen />} />
      <Route path="/take-parcel/:admin" element={<TakePackageScreen />} />

      <Route path="*" element={<ShipmentsScreen />} />
    </Routes>
  )
}

export default OperatorRouter
