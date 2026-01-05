/* eslint-disable prettier/prettier */

import styled from '@emotion/styled'
import Tabs, { Tab } from '@ui/components/tabs'
import ScreenContainer from '@ui/components/screen-container'
import { useTranslation } from '@domain/administration/stores'
import ScreenBreadcrumbs from '@ui/components/screen-breadcrumbs'
import { Fragment, useState } from 'react'
import { isEmpty } from 'lodash'
import DailyMedia from '@ui/modules/reports/daily-media'
import EndOfShift from '@ui/modules/reports/end-of-shift'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  gap: 1rem;
  height: 100%;
  overflow: hidden;

  & .header {
    & .welcome-section {
      display: flex;
      justify-content: space-between;

      & .info {
        display: flex;
        flex-direction: column;
        text-align: end;
        font-family: Poppins;
        font-weight: 700;
        font-size: 2.5rem;
        line-height: 2.5rem;
        letter-spacing: 0.003rem;
        & .message {
          font-weight: 600;
          font-size: 1rem;
          line-height: 1.125rem;
          text-transform: uppercase;
          color: var(--secondary-dark);
        }
      }
    }
  }

  & .tabs {
    margin-top: 4.25rem;
  }

  & .content {
    height: 100%;
    overflow: auto;
    padding-right: 1rem;
  }
`

const ReportTabs = [
  {
    id: 0,
    label: 'Daily media',
    path: 'daily-media',
    default: true,
    renderComponent: () => <DailyMedia />
  },
  {
    id: 1,
    label: 'End of shift',
    path: 'end-of-shift',
    default: false,
    renderComponent: () => <EndOfShift />
  }
]

const ReportsScreen = () => {
  const { t } = useTranslation()

  const [activeTab, setActiveTab] = useState()

  const isTabActive = (path) => {
    if (!isEmpty(activeTab)) {
      return path === activeTab
    }

    const defaultTab = ReportTabs.find((t) => t.default)

    return defaultTab?.path === path
  }

  const getActiveTabName = () => {
    let currentPath = activeTab

    if (isEmpty(activeTab)) {
      const defaultTab = ReportTabs.find((t) => t.default)
      currentPath = defaultTab?.path
    }

    const activeTabData = ReportTabs.find((tab) => tab.path === currentPath)
    return activeTabData ? t(activeTabData.label) : t('Daily media')
  }

  return (
    <ScreenContainer isAdmin={true} overflow={false} padding={false} hasLogo={false}>
      <Container>
        <div className="header">
          <ScreenBreadcrumbs names={[t('Reports'), getActiveTabName()]} />

          <Tabs className="tabs">
            <>
              {ReportTabs.map((x) => (
                <Tab
                  key={`report-tab-${x.id}`}
                  active={isTabActive(x.path)}
                  name={t(x.label)}
                  onSelect={() => {
                    setActiveTab(x.path)
                  }}
                />
              ))}
            </>
          </Tabs>
        </div>

        <div className="content">
          {ReportTabs.map((x) => (
            <Fragment key={`report-tab-${x.id}`}>
              {isTabActive(x.path) && (
                <Fragment key={`report-content-${x.id}`}>{x.renderComponent()}</Fragment>
              )}
            </Fragment>
          ))}
        </div>
      </Container>
    </ScreenContainer>
  )
}

export default ReportsScreen
