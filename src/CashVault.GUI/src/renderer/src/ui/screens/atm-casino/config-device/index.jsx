/* eslint-disable prettier/prettier */
import ScreenContainer from '@ui/components/screen-container'
import styled from '@emotion/styled'
import { useTranslation } from '@domain/administration/stores'
import Tabs, { Tab } from '@ui/components/tabs'
import ConfigDeviceMain from '@ui/modules/config-device/main'
import { useState } from 'react'
import RegionalDeviceConfig from '@ui/modules/config-device/regional'
import ServerDeviceConfig from '@ui/modules/config-device/server'
import NetworkDeviceConfig from '@ui/modules/config-device/network'
import OnlineDeviceConfig from '@ui/modules/config-device/online-config'
import ScreenBreadcrumbs from '@ui/components/screen-breadcrumbs'

// ConfigDeviceScreen.jsx

const Container = styled.div`
  display: flex;
  flex-direction: column;
  gap: 1rem;
  height: 100%;

  & .content {
    flex: 1;
    overflow-y: auto;
    display: flex;
    flex-direction: column;

    padding-right: 1rem;
  }

  & .tabs {
    padding-top: 2rem;
    flex-shrink: 0;
  }
`

const MAIN_TAB = 1
const REGIONAL_TAB = 2
const SERVER_TAB = 3
const NETWORK_TAB = 4
const ONLINE_INTEGRATIONS_TAB = 5

const ConfigDeviceScreen = () => {
  const { t } = useTranslation()
  const [activeTab, setActiveTabe] = useState(MAIN_TAB)

  const getActiveTabName = () => {
    switch (activeTab) {
      case MAIN_TAB:
        return t('Main')
      case REGIONAL_TAB:
        return t('Regional')
      case SERVER_TAB:
        return t('Server')
      case NETWORK_TAB:
        return t('Network')
      case ONLINE_INTEGRATIONS_TAB:
        return t('Online integrations')
      default:
        return t('Main')
    }
  }

  return (
    <ScreenContainer isAdmin={true} overflow={false} padding={false} hasLogo={false}>
      <Container>
        <ScreenBreadcrumbs names={[t('Config device'), getActiveTabName()]} />

        <Tabs className="tabs">
          <Tab
            active={activeTab === MAIN_TAB}
            onSelect={() => setActiveTabe(MAIN_TAB)}
            name={t('Main')}
          />
          <Tab
            active={activeTab === REGIONAL_TAB}
            onSelect={() => setActiveTabe(REGIONAL_TAB)}
            name={t('Regional')}
          />
          <Tab
            active={activeTab === SERVER_TAB}
            onSelect={() => setActiveTabe(SERVER_TAB)}
            name={t('Server')}
          />
          <Tab
            active={activeTab === NETWORK_TAB}
            onSelect={() => setActiveTabe(NETWORK_TAB)}
            name={t('Network')}
          />
          <Tab
            active={activeTab === ONLINE_INTEGRATIONS_TAB}
            onSelect={() => setActiveTabe(ONLINE_INTEGRATIONS_TAB)}
            name={t('Online integrations')}
          />
        </Tabs>

        <div className="content">
          {activeTab === MAIN_TAB && <ConfigDeviceMain />}
          {activeTab === REGIONAL_TAB && <RegionalDeviceConfig />}
          {activeTab === SERVER_TAB && <ServerDeviceConfig />}
          {activeTab === NETWORK_TAB && <NetworkDeviceConfig />}
          {activeTab === ONLINE_INTEGRATIONS_TAB && <OnlineDeviceConfig />}
        </div>
      </Container>
    </ScreenContainer>
  )
}

export default ConfigDeviceScreen
