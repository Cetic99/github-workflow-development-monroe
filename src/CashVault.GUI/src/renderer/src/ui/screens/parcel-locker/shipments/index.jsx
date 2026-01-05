/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import { useTranslation } from '@src/app/domain/administration/stores'
import Button from '@ui/components/button'
import IconEdit from '@ui/components/icons/IconEdit'
import Table from '@ui/components/table'
import ScreenContainerTop from '@ui/layouts/screen-container-top'
import Tabs, { Tab } from '@ui/components/tabs'
import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { usePendingShipments, useShipmentsInLockers } from '@src/app/domain/parcel-locker/queries'
import { useCourier, usePostalService } from '@src/app/domain/parcel-locker/stores/parcel-store'
import { displayAmountWithCurrency, parseDateTime } from '@src/app/domain/parcel-locker/services'
import BarLoader from '@ui/components/bar-loader'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  gap: 2rem;
  padding: 4rem 2rem 6rem 2rem;

  & .buttons {
    display: flex;
    justify-content: flex-end;
    gap: 1rem;
  }
`

const ShipmentsScreen = () => {
  const { t } = useTranslation()

  const navigate = useNavigate()

  const { id: courierId } = useCourier()
  const postalService = usePostalService()

  const [activeTab, setActiveTab] = useState('lockers')

  const { data: parcelsInLockers, isLoading: parcelsInLockersLoading } = useShipmentsInLockers(
    courierId,
    postalService,
    activeTab === 'lockers'
  )

  const { data: pendingParcels, isLoading: pendingParcelsLoading } = usePendingShipments(
    courierId,
    postalService,
    activeTab !== 'lockers'
  )

  const isLoading = activeTab == 'lockers' ? parcelsInLockersLoading : pendingParcelsLoading

  const columns = [
    { id: 1, width: '15%', value: t('Barcode'), accessor: 'barcode', textAlign: 'left' },
    { id: 2, width: '5%', value: t('Size'), accessor: 'parcelLockerSize', textAlign: 'left' },
    { id: 3, width: '15%', value: t('Phone'), accessor: 'phoneNumber', textAlign: 'left' },
    {
      id: 4,
      width: '15%',
      value: t('Amount'),
      render: (rowData) => {
        if (rowData?.paymentRequired === true) {
          return `${displayAmountWithCurrency(rowData?.amount, rowData?.currency)}`
        }

        return t('No payment required')
      },
      textAlign: 'left'
    },
    {
      id: 5,
      width: '15%',
      value: t('Expiry Date'),
      render: (rowData) => parseDateTime(rowData?.expirationDate),
      textAlign: 'left'
    },
    {
      id: 6,
      width: '10%',
      value: '',
      textAlign: 'right',
      render: () => (
        <div>
          <IconEdit size="m" />
        </div>
      )
    }
  ]

  const handlePickUpShipment = () => {
    navigate('/parcel-locker/get-parcel')
  }

  const handleAddShipment = () => {
    navigate('/parcel-locker/scan-parcel/barcode')
  }

  const rowColor = []

  return (
    <ScreenContainerTop>
      <Container>
        <div className="buttons">
          {activeTab === 'lockers' && (
            <Button onClick={handlePickUpShipment}>{t('Get parcel')}</Button>
          )}
          {activeTab === 'pending' && (
            <Button onClick={handleAddShipment}>{t('Deposit parcel')}</Button>
          )}
        </div>

        <Tabs className="tabs">
          <Tab
            active={activeTab === 'lockers'}
            name={t('Parcels in lockers')}
            onSelect={() => setActiveTab('lockers')}
          />
          <Tab
            active={activeTab === 'pending'}
            name={t('Pending parcels')}
            onSelect={() => setActiveTab('pending')}
          />
        </Tabs>
        <BarLoader loading={isLoading} />
        {!isLoading && (
          <Table
            columns={columns}
            data={activeTab === 'lockers' ? parcelsInLockers : pendingParcels}
            rowColor={rowColor}
          />
        )}
      </Container>
    </ScreenContainerTop>
  )
}

export default ShipmentsScreen
