/* eslint-disable prettier/prettier */
import { useTranslation } from '@domain/administration/stores'
import styled from '@emotion/styled'
import ScreenContainer from '@ui/components/screen-container'
import Tabs, { Tab } from '@ui/components/tabs'
import { useState } from 'react'
import ScreenBreadcrumbs from '@ui/components/screen-breadcrumbs'
import Diagnostic from '@ui/modules/maintenance/diagnostic'
import TransactionLogs from '@ui/modules/server-logs/transactions'
import EventFailLogs from '@ui/modules/server-logs/fail'
import CircleButton from '@ui/components/circle-button'
import { useNavigate } from 'react-router-dom'
import IconLeftHalfArrow from '@ui/components/icons/IconLeftHalfArrow'
import Authorized from '@ui/components/authorized'
import { Permission } from '@domain/global/constants'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  gap: 1rem;
  height: 100%;
  overflow: hidden;

  & .content {
    display: flex;
    flex-direction: column;
    gap: 1rem;
    height: 100%;
    overflow: auto;
    padding-right: 1rem;
  }

  & .tabs {
    padding-top: 2rem;
  }

  & .back-action {
    margin-top: auto;
    padding: 0.5rem 0 0 0;
    position: sticky;
    bottom: 0;
    z-index: 10;

    pointer-events: none;

    & > * {
      pointer-events: all;
    }
  }
`

const DIAGNOSTIC_TAB = 1
const TRANSACTIONS_TAB = 2
const FAIL_TAB = 3

const MaintenanceScreen = () => {
  const { t } = useTranslation()
  const [activeTab, setActiveTabe] = useState(DIAGNOSTIC_TAB)
  const navigate = useNavigate()

  const getActiveTabName = () => {
    switch (activeTab) {
      case DIAGNOSTIC_TAB:
        return t('Diagnostics')
      case TRANSACTIONS_TAB:
        return t('Transaction')
      case FAIL_TAB:
        return t('Fail')
      default:
        return t('Diagnostics')
    }
  }

  //===========================================================================

  return (
    <ScreenContainer isAdmin={true} overflow={false} padding={false} hasLogo={false}>
      <Container>
        <div className="maintenance-header">
          <ScreenBreadcrumbs names={[t('Maintenance'), getActiveTabName()]} />
          <Tabs className="tabs">
            <Tab
              active={activeTab === DIAGNOSTIC_TAB}
              onSelect={() => setActiveTabe(DIAGNOSTIC_TAB)}
              name={t('Diagnostics')}
            />
            <Authorized permissions={[Permission.Logs]}>
              <Tab
                active={activeTab === TRANSACTIONS_TAB}
                onSelect={() => setActiveTabe(TRANSACTIONS_TAB)}
                name={t('Transaction')}
              />
              <Tab
                active={activeTab === FAIL_TAB}
                onSelect={() => setActiveTabe(FAIL_TAB)}
                name={t('Fail')}
              />
            </Authorized>
          </Tabs>
        </div>

        <div className="content">
          {activeTab === DIAGNOSTIC_TAB && <Diagnostic />}{' '}
          {activeTab === TRANSACTIONS_TAB && <TransactionLogs />}
          {activeTab === FAIL_TAB && <EventFailLogs />}
          <div className="back-action">
            <CircleButton
              size="l"
              textRight={t('Back')}
              icon={(props) => <IconLeftHalfArrow {...props} />}
              onClick={() => navigate('/')}
              shadow={true}
            />
          </div>
        </div>
      </Container>
    </ScreenContainer>
  )
}

export default MaintenanceScreen
