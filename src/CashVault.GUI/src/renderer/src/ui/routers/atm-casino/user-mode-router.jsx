/* eslint-disable prettier/prettier */
import { Route, Routes } from 'react-router-dom'
import UserMainScreen from '@screens/atm-casino/user-main'
import AdminLoginScreen from '@screens/shared/admin-login'
import SelectLanguageScreen from '@screens/shared/select-language'
import PrintTicket from '@ui/modules/user-main/print-ticket'
import WithdrawMoney from '@ui/modules/user-main/withdraw-money'
import PayoutProcessing from '@ui/modules/user-main/payout-processing'
import BetboxTicket from '@ui/modules/user-main/betbox-ticket'
import SuccessScreen from '@ui/screens/atm-casino/success'

const UserRouter = () => {
  return (
    <Routes>
      <Route path="/" element={<UserMainScreen />} />
      <Route path="language" element={<SelectLanguageScreen />} />
      <Route path="print-ticket" element={<PrintTicket />} />
      <Route path="withdraw-money" element={<WithdrawMoney />} />
      <Route path="payout-processing" element={<PayoutProcessing />} />
      <Route path="admin-login" element={<AdminLoginScreen />} />
      <Route path="betbox-ticket" element={<BetboxTicket />} />
      <Route path="success" element={<SuccessScreen />} />
      <Route path="*" element={<UserMainScreen />} />
    </Routes>
  )
}

export default UserRouter
