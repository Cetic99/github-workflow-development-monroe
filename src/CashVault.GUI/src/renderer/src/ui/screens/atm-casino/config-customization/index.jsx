/* eslint-disable prettier/prettier */

import { useTranslation } from '@domain/administration/stores'

import { Fragment, useState } from 'react'
import styled from '@emotion/styled'
import { isEmpty } from 'lodash'

import Tabs, { Tab } from '@ui/components/tabs'
import ScreenContainer from '@ui/components/screen-container'
import ScreenBreadcrumbs from '@ui/components/screen-breadcrumbs'
import UserWidgetsScreen from '@ui/modules/config-customization/user-widgets'

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
  }
`

const CustomizationTabs = [
  {
    id: 0,
    label: 'User widgets',
    path: 'user-widgets',
    default: true,
    renderComponent: () => <UserWidgetsScreen />
  }
]

const ConfigCustomizationScreen = () => {
  const { t } = useTranslation()

  const [activeTab, setActiveTab] = useState()

  const isTabActive = (path) => {
    if (!isEmpty(activeTab)) {
      return path === activeTab
    }

    const defaultTab = CustomizationTabs.find((t) => t.default)

    return defaultTab?.path === path
  }

  return (
    <ScreenContainer isAdmin={true} overflow={false} padding={false} hasLogo={false}>
      <Container>
        <div className="header">
          <ScreenBreadcrumbs names={[t('Customization')]} />

          <Tabs className="tabs">
            {CustomizationTabs.map((x, i) => (
              <Tab
                key={`customization-tab-${i}`}
                active={isTabActive(x.path)}
                name={t(x.label)}
                onSelect={() => {
                  setActiveTab(x.path)
                }}
              />
            ))}
          </Tabs>
        </div>

        <div className="content">
          {CustomizationTabs.map((x, i) => (
            <>
              {isTabActive(x.path) && (
                <Fragment key={`customization-content-${i}`}>{x.renderComponent()}</Fragment>
              )}
            </>
          ))}
        </div>
      </Container>
    </ScreenContainer>
  )
}

export default ConfigCustomizationScreen
