/* eslint-disable prettier/prettier */

import styled from '@emotion/styled'
import Tabs, { Tab } from '@ui/components/tabs'
import ScreenContainer from '@ui/components/screen-container'
import { useTranslation } from '@domain/administration/stores'
import { Fragment, useState } from 'react'

import PayoutRules from '@ui/modules/administration/payout-rules'
import Operators from '@ui/modules/administration/operators'
import Messages from '@ui/modules/administration/messages'
import { isEmpty } from 'lodash'
import ScreenBreadcrumbs from '@ui/components/screen-breadcrumbs'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  gap: 1rem;
  height: 100%;
  overflow: hidden;

  & .tabs {
    margin-top: 4.25rem;
  }

  & .administration-content {
    height: 100%;
    overflow: auto;
    padding-right: 1rem;
  }
`

const AdministrationScreenTabs = [
  {
    id: 0,
    label: 'Payout rules',
    path: 'payout-rules',
    default: true,
    renderComponent: () => <PayoutRules />
  },
  {
    id: 1,
    label: 'Operators',
    path: 'operators',
    default: false,
    renderComponent: (props) => <Operators {...props} />
  },
  {
    id: 2,
    label: 'Messages',
    path: 'messages',
    default: false,
    renderComponent: () => <Messages />
  }
]

const Administration = () => {
  const { t } = useTranslation()

  const [activeTab, setActiveTab] = useState()
  const [hideTabs, setHideTabs] = useState(false)

  const isTabActive = (path) => {
    if (!isEmpty(activeTab)) {
      return path === activeTab
    }

    const defaultTab = AdministrationScreenTabs.find((t) => t.default)

    return defaultTab?.path === path
  }

  const getActiveTabName = () => {
    let currentPath = activeTab

    if (isEmpty(activeTab)) {
      const defaultTab = AdministrationScreenTabs.find((t) => t.default)
      currentPath = defaultTab?.path
    }

    const activeTabData = AdministrationScreenTabs.find((tab) => tab.path === currentPath)
    return activeTabData ? t(activeTabData.label) : t('Payout rules')
  }

  return (
    <ScreenContainer isAdmin={true} overflow={false} padding={false} hasLogo={false}>
      <Container>
        <div className="administration-header">
          <ScreenBreadcrumbs names={[t('Administration'), getActiveTabName()]} />

          {!hideTabs && (
            <Tabs className="tabs">
              <>
                {AdministrationScreenTabs.map((x, i) => (
                  <Tab
                    key={`admin-tab-${x.label}`}
                    active={isTabActive(x.path)}
                    name={t(x.label)}
                    onSelect={() => {
                      setActiveTab(x.path)
                    }}
                  />
                ))}
              </>
            </Tabs>
          )}
        </div>

        <div className="administration-content">
          {AdministrationScreenTabs.map((x, i) => (
            <Fragment key={`administration-tab-${x.label}`}>
              {isTabActive(x.path) && (
                <Fragment key={`administration-content-${x.label}`}>
                  {x.renderComponent({ setHideTabs })}
                </Fragment>
              )}
            </Fragment>
          ))}
        </div>
      </Container>
    </ScreenContainer>
  )
}

export default Administration
