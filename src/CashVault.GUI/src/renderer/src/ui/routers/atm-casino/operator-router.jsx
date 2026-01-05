/* eslint-disable prettier/prettier */
import { Route, Routes } from 'react-router-dom'

import SafeModeScreen from '@ui/screens/safe-mode'

import SelectLanguageScreen from '@screens/shared/select-language'
import LogoutScreen from '@screens/shared/logout'
import OperatorMenuScreen from '@screens/atm-casino/operator-menu'
import ConfigDeviceScreen from '@screens/atm-casino/config-device'
import MoneyServiceScreen from '@screens/atm-casino/money-service'
import MaintenanceScreen from '@screens/atm-casino/maintenance'
import MasterLoginScreen from '@screens/atm-casino/master-login'
import Administration from '@screens/atm-casino/administration'
import MoneyConfigScreen from '@screens/atm-casino/config-money'
import ServerLogsScreen from '@screens/atm-casino/server-logs'
import ReportsScreen from '@screens/atm-casino/reports'
import DeviceDiagnosticScreen from '@ui/screens/atm-casino/device-diagnostic'
import ConfigCustomizationScreen from '@ui/screens/atm-casino/config-customization'

const OperatorRouter = () => {
  return (
    <Routes>
      <Route path="safe-mode" element={<SafeModeScreen />} />
      <Route path="/" element={<OperatorMenuScreen />} />
      <Route path="language" element={<SelectLanguageScreen />} />
      <Route path="logout" element={<LogoutScreen />} />
      <Route path="master-auth" element={<MasterLoginScreen />} />

      <Route path="money-service" element={<MoneyServiceScreen />} />
      <Route path="maintenance" element={<MaintenanceScreen />} />
      <Route path="administration" element={<Administration />} />

      <Route path="configuration-devices" element={<ConfigDeviceScreen />} />
      <Route path="configuration-money" element={<MoneyConfigScreen />} />
      <Route path="logs" element={<ServerLogsScreen />} />
      <Route path="reports" element={<ReportsScreen />} />
      <Route path="configuration-customization" element={<ConfigCustomizationScreen />} />

      <Route path="device-diagnostic/:type" element={<DeviceDiagnosticScreen />} />
      {/* //====================================== */}

      <Route path="*" element={<OperatorMenuScreen />} />
    </Routes>
  )
}

export default OperatorRouter
