/* eslint-disable prettier/prettier */
import { Route, Routes } from 'react-router-dom'
import SafeModePage from '@ui/components/safe-mode-page'
import SafeModeScreen from '@ui/screens/safe-mode'

const SafeRouter = () => {
  return (
    <Routes>
      <Route path="/" element={<SafeModePage />} />
      <Route path="safe-mode" element={<SafeModeScreen />} />
      <Route path="*" element={<SafeModePage />} />
    </Routes>
  )
}

export default SafeRouter
