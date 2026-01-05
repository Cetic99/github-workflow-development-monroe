/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import 'react-simple-keyboard/build/css/index.css'
import { useEffect } from 'react'
import { useLoading } from '@domain/global/stores'
import useAuthentication from '@domain/global/hooks/use-authentication'
import { useGetActiveDevices } from '@domain/administration/queries'
import { deviceStore } from '@domain/device/stores'
import BillTicketAcceptingOverlay from '@ui/components/bill-ticket-accepting-modal'
import FullPageLoader from '@ui/components/full-page-loader'
import { useIdleScreen } from '@src/app/domain/global/stores/idle'
import { useNavigate } from 'react-router-dom'

const Main = styled.main`
  width: 100dvw;
  height: 100dvh;
  display: flex;
  flex-wrap: nowrap;
`

const Content = styled.div`
  flex-grow: 100;
  height: 100dvh;
  display: flex;
  align-items: center;
  justify-content: center;
`

const MainLayout = ({ children }) => {
  useAuthentication()
  const navigate = useNavigate()
  useIdleScreen(navigate)
  const loading = useLoading()

  const query = useGetActiveDevices()

  useEffect(() => {
    if (query.data) {
      deviceStore.getState().initialize(query?.data)
    }
  }, [query?.data, query.isLoading])

  return (
    <Main className="main-layout">
      <BillTicketAcceptingOverlay />

      <FullPageLoader loading={loading} />
      <Content className="main-content">{children}</Content>
    </Main>
  )
}

export default MainLayout
