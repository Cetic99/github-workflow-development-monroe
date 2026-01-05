/* eslint-disable prettier/prettier */
import { Route, Routes } from 'react-router-dom'

import IdleScreen from '@screens/atm/idle'
import SelectLanguageScreen from '@screens/atm/select-language'
import PinInputScreen from '@screens/atm/pin-input'
import UserMainScreen from '@screens/atm/user-main'
import SettingsScreen from '@screens/atm/settings'
import TakeCardScreen from '@screens/atm/take-card'
import ChooseAmountScreen from '@screens/atm/choose-amount'
import SelectReceiptScreen from '@screens/atm/select-receipt'
import LoadingScreen from '@screens/atm/loading'
import TakeMoneyScreen from '@screens/atm/take-money'
import CheckBalanceScreen from '@screens/atm/check-balance'
import BalanceScreen from '@screens/atm/balance'
import ChangePinScreen from '@screens/atm/change-pin'
import ChangedPinScreen from '@screens/atm/changed-pin'
import DepositScreen from '@screens/atm/deposit'
import DepositAmountScreen from '@screens/atm/deposit-amount'
import TransactionCompleteScreen from '@screens/atm/transaction-complete'
import UnblockPinScreen from '@screens/atm/unblock-pin'
import CardlessTransactionScreen from '@screens/atm/cardless-transaction'
import CardlessCodeScreen from '@screens/atm/cardless-code'
import CardlessAmountScreen from '@screens/atm/cardless-amount'
import TakeReceiptScreen from '@screens/atm/take-receipt'

const AtmUserRouter = () => {
  return (
    <Routes>
      <Route path="atm/" element={<UserMainScreen />} />
      <Route path="atm/idle" element={<IdleScreen />} />
      <Route path="atm/select-language" element={<SelectLanguageScreen />} />
      <Route path="atm/settings" element={<SettingsScreen />} />
      <Route path="atm/take-card/:money" element={<TakeCardScreen />} />
      <Route path="atm/take-money" element={<TakeMoneyScreen />} />
      <Route path="atm/choose-amount" element={<ChooseAmountScreen />} />

      <Route path="atm/take-receipt" element={<TakeReceiptScreen />} />
      <Route path="atm/select-receipt" element={<SelectReceiptScreen />} />

      <Route path="atm/check-balance" element={<CheckBalanceScreen />} />
      <Route path="atm/balance" element={<BalanceScreen />} />

      <Route path="atm/pin-input" element={<PinInputScreen />} />
      <Route path="atm/change-pin" element={<ChangePinScreen />} />
      <Route path="atm/changed-pin" element={<ChangedPinScreen />} />
      <Route path="atm/unblock-pin" element={<UnblockPinScreen />} />

      <Route path="atm/deposit" element={<DepositScreen />} />
      <Route path="atm/deposit/amount" element={<DepositAmountScreen />} />

      <Route path="atm/cardless-transaction" element={<CardlessTransactionScreen />} />
      <Route path="atm/cardless-transaction/code" element={<CardlessCodeScreen />} />
      <Route path="atm/cardless-transaction/amount" element={<CardlessAmountScreen />} />

      <Route path="atm/transaction-complete" element={<TransactionCompleteScreen />} />
      <Route path="atm/loading" element={<LoadingScreen />} />
      <Route path="*" element={<UserMainScreen />} />
    </Routes>
  )
}

export default AtmUserRouter
