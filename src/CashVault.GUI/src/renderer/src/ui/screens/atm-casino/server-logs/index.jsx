/* eslint-disable prettier/prettier */
import { useTranslation } from '@domain/administration/stores'
import styled from '@emotion/styled'
import ScreenContainer from '@ui/components/screen-container'
import Tabs, { Tab } from '@ui/components/tabs'
import { useState } from 'react'
import TransactionLogs from '@ui/modules/server-logs/transactions'
import ScreenBreadcrumbs from '@ui/components/screen-breadcrumbs'
import EventLogs from '@ui/modules/server-logs/events'
import CircleButton from '@ui/components/circle-button'
import IconLeftHalfArrow from '@ui/components/icons/IconLeftHalfArrow'
import { useNavigate } from 'react-router-dom'
import EventFailLogs from '@ui/modules/server-logs/fail'
import TicketLogs from '@ui/modules/server-logs/ticket'
import MoneyStatusLogs from '@ui/modules/server-logs/money-status'

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

  & .filter {
    padding: 1rem 0;
    display: flex;
    flex-direction: row-reverse;
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

  & .tabs {
    padding-top: 2rem;
  }
`

const EVENTS_TAB = 1
const TRANSACTIONS_TAB = 2
const FAIL_TAB = 3
const TICKET_TAB = 4
const MONEY_STATUS_TAB = 5

const ServerLogsScreen = () => {
  const { t } = useTranslation()
  const navigate = useNavigate()
  const [activeTab, setActiveTabe] = useState(1)

  const handleOnGoBack = () => {
    navigate('/')
  }

  const getActiveTabName = () => {
    switch (activeTab) {
      case EVENTS_TAB:
        return t('Event')
      case TRANSACTIONS_TAB:
        return t('Transaction')
      case FAIL_TAB:
        return t('Fail')
      case TICKET_TAB:
        return t('Ticket')
      case MONEY_STATUS_TAB:
        return t('Money Status')
      default:
        return t('Event')
    }
  }

  //===========================================================================

  return (
    <ScreenContainer isAdmin={true} overflow={false} padding={false} hasLogo={false}>
      <Container>
        <div className="logs-header">
          <ScreenBreadcrumbs names={[t('Logs'), getActiveTabName()]} />

          <Tabs className="tabs">
            <Tab
              active={activeTab === EVENTS_TAB}
              onSelect={() => setActiveTabe(EVENTS_TAB)}
              name={t('Event')}
            />
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
            <Tab
              active={activeTab === TICKET_TAB}
              onSelect={() => setActiveTabe(TICKET_TAB)}
              name={t('Ticket')}
            />
            <Tab
              active={activeTab === MONEY_STATUS_TAB}
              onSelect={() => setActiveTabe(MONEY_STATUS_TAB)}
              name={t('Money Status')}
            />
          </Tabs>
        </div>

        <div className="content">
          {activeTab === TRANSACTIONS_TAB && <TransactionLogs />}
          {activeTab === EVENTS_TAB && <EventLogs />}
          {activeTab === FAIL_TAB && <EventFailLogs />}
          {activeTab === TICKET_TAB && <TicketLogs />}
          {activeTab === MONEY_STATUS_TAB && <MoneyStatusLogs />}
          <div className="back-action">
            <CircleButton
              size="l"
              textRight={t('Back')}
              icon={(props) => <IconLeftHalfArrow {...props} />}
              onClick={() => handleOnGoBack()}
              shadow={true}
            />
          </div>
        </div>
      </Container>
    </ScreenContainer>
  )
}

export default ServerLogsScreen
