/* eslint-disable prettier/prettier */
import { useState } from 'react'
import styled from '@emotion/styled'
import { useHeartbeat } from '@domain/global/stores'
import LogFile from '@ui/components/log-file'
import ScriptFile from '@ui/components/script-file'
import useLogs from '@domain/global/hooks/use-logs'
import useScripts from '@domain/global/hooks/use-scripts'
import { useSettings } from '@domain/global/hooks/use-settings'
import AppSettingsModal from '@ui/components/app-settings-modal'
import Button from '@ui/components/button'
import { useTranslation } from '@domain/administration/stores'
import ScreenContainer from '@ui/components/screen-container'
import Alert from '@ui/components/alert'
import TextInput from '@ui/components/inputs/text-input'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  gap: 2rem;
  padding-bottom: 2.5rem;
  margin-top: 5rem;
  margin-bottom: 2rem;

  & .list {
    display: flex;
    flex-direction: column;
    gap: 1rem;
  }

  & .list-header {
    padding: 0.5rem;
    display: flex;
    gap: 1rem;
    font-size: 1rem;
    font-weight: bold;
  }

  & .list-content {
    display: flex;
    flex-direction: column;
    gap: 0.75rem;
  }

  & .not-items {
    padding-left: 0.25rem;
  }

  & .settings-actions {
    display: flex;
    gap: 1.5rem;
  }
`

const SafeModeScreen = () => {
  const connected = useHeartbeat()

  const { logFiles, onLogsSearchChange, onFileSelect, refreshLogs } = useLogs()
  const { scriptFiles, onScriptSelect, onScriptsSearchChange, refreshScripts } = useScripts()
  const [openSettings, setOpenSettings] = useState(false)

  const { t } = useTranslation()

  const [settings, updateSettings] = useSettings()

  const handleOnSettingsSave = (settings) => {
    updateSettings(settings)
    setOpenSettings(false)
    handleRefresh()
  }

  const handleRefresh = () => {
    refreshLogs()
    refreshScripts()
  }

  const handleExit = () => {
    window.general.exit()
  }

  return (
    <ScreenContainer isAdmin={true} overflow={false} padding={false} hasLogo={true}>
      <Container>
        <Alert
          severity={connected ? 'info' : 'error'}
          text={connected ? 'Connected to localhost' : 'No connection to localhost'}
        />
        <div className="settings-actions">
          <Button onClick={() => setOpenSettings(true)}>{t('Settings')}</Button>

          <Button onClick={handleExit}>{t('Close app')}</Button>

          <Button onClick={handleRefresh}>{t('Refresh')}</Button>
        </div>
        <div className="list">
          <TextInput label={t('Search scripts')} onChange={onScriptsSearchChange} />
          <div className="list-content">
            {scriptFiles?.map((f, i) => (
              <ScriptFile key={i} scriptName={f} onSelect={() => onScriptSelect(f)} />
            ))}

            {scriptFiles?.length === 0 && <div className="not-items">{t('No scripts found')}</div>}
          </div>
        </div>
        <div className="list">
          <TextInput label={t('Search logs')} onChange={onLogsSearchChange} />

          <div className="list-content">
            {logFiles?.map((f, i) => (
              <LogFile key={i} fileName={f} onSelect={() => onFileSelect(f)} />
            ))}

            {logFiles?.length === 0 && <div className="not-items">{t('No logs found')}</div>}
          </div>
        </div>

        <AppSettingsModal
          open={openSettings}
          onClose={() => setOpenSettings(false)}
          settings={settings}
          onSave={handleOnSettingsSave}
        />
      </Container>
    </ScreenContainer>
  )
}

export default SafeModeScreen
