/* eslint-disable prettier/prettier */
import ScreenContainer from '@ui/components/screen-container'
import styled from '@emotion/styled'
import { useTranslation } from '@domain/administration/stores'
import Tabs, { Tab } from '@ui/components/tabs'
import { useEffect, useMemo, useState } from 'react'
import MoneyConfigBillDispenser from '@ui/modules/config-money/bill-dispenser'
import MoneyConfigBillAcceptor from '@ui/modules/config-money/bill-acceptor'
import MoneyConfigTITOPrinter from '@ui/modules/config-money/tito-printer'
import ScreenBreadcrumbs from '@ui/components/screen-breadcrumbs'
import { DEVICE_TYPE } from '@src/app/domain/device/constants'
import { isDeviceSupported } from '@src/app/domain/global/stores'
import MoneyConfigCoinAcceptor from '@ui/modules/config-money/coin-acceptor'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  gap: 1rem;
  height: 100%;
  overflow: hidden;

  & .content {
    margin-top: 2rem;
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
`

const MoneyConfigScreen = () => {
  const { t } = useTranslation()

  const allTabs = useMemo(
    () => [
      {
        id: 'billDispenser',
        device: DEVICE_TYPE.BILL_DISPENSER,
        label: 'Bill dispenser',
        component: <MoneyConfigBillDispenser />
      },
      {
        id: 'titoPrinter',
        device: DEVICE_TYPE.TITO_PRINTER,
        label: 'Ticket printer',
        component: <MoneyConfigTITOPrinter />
      },
      {
        id: 'billAcceptor',
        device: DEVICE_TYPE.BILL_ACCEPTOR,
        label: 'Bill acceptor',
        component: <MoneyConfigBillAcceptor />
      },
      {
        id: 'coinacceptor',
        device: DEVICE_TYPE.COIN_ACCEPTOR,
        label: 'Coin acceptor',
        component: <MoneyConfigCoinAcceptor />
      }
    ],
    [t]
  )

  const visibleTabs = allTabs.filter((t) => isDeviceSupported(t.device))

  const [activeTab, setActiveTab] = useState(visibleTabs[0]?.id)

  useEffect(() => {
    if (!visibleTabs.some((t) => t.id === activeTab)) {
      setActiveTab(visibleTabs[0]?.id)
    }
  }, [visibleTabs])

  const activeTabObj = visibleTabs.find((t) => t.id === activeTab)

  return (
    <ScreenContainer isAdmin={true} overflow={false} padding={false} hasLogo={false}>
      <Container>
        <ScreenBreadcrumbs names={[t('Money'), activeTabObj?.label ?? '']} />

        <Tabs className="tabs">
          {visibleTabs.map((tab) => (
            <Tab
              key={tab.id}
              active={activeTab === tab.id}
              onSelect={() => setActiveTab(tab.id)}
              name={tab.label}
            />
          ))}
        </Tabs>

        <div className="content">{activeTabObj?.component}</div>
      </Container>
    </ScreenContainer>
  )
}

export default MoneyConfigScreen
