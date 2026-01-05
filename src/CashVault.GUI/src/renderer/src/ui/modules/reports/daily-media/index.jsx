/* eslint-disable prettier/prettier */

import styled from '@emotion/styled'
import { useTranslation } from '@domain/administration/stores'
import { DEVICE_TYPE } from '@domain/device/constants'
import { useGetDeviceByType } from '@domain/device/stores'
import { useDailyMediaReport } from '@domain/reports/queries'
import { usePrintDailyMediaReport } from '@domain/reports/commands'
import { useNavigate } from 'react-router-dom'

import IconPrinter from '@ui/components/icons/IconPrinter'
import IconLeftHalfArrow from '@ui/components/icons/IconLeftHalfArrow'

import Button from '@ui/components/button'
import DateInput from '@ui/components/inputs/date-input'
import Divider from '@ui/components/divider'
import CircleButton from '@ui/components/circle-button'
import DecimalInput from '@ui/components/inputs/decimal-input'
import Alert from '@ui/components/alert'
import { severityMap } from '@ui/components/alert/severity'
import FullPageLoader from '@ui/components/full-page-loader'

import { useEffect, useState } from 'react'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  gap: 2.75rem;
  height: 100%;

  & .actions {
    display: flex;
    flex-direction: row-reverse;
    gap: 1rem;

    & .alert {
      width: 100%;
    }
  }

  & .full-width {
    width: 100%;
  }

  & .single-row {
    > div {
      width: 100%;
    }
  }

  & .row {
    display: flex;
    gap: 1rem;

    > div {
      width: 100%;
    }
  }

  & .section {
    display: flex;
    flex-direction: column;
    gap: 1rem;

    & .title {
      font-family: Poppins;
      font-weight: 600;
      font-size: 1.75rem;
      line-height: 1.875rem;
      letter-spacing: -0.001rem;

      padding-bottom: 1.25rem;
    }
  }

  & .footer {
    margin-top: auto;
    display: flex;
    width: 100%;
    justify-content: space-between;
    position: sticky;
    bottom: 0;
    z-index: 10;
    pointer-events: none;

    & > * {
      pointer-events: all;
    }
  }
`

const DailyMedia = () => {
  const { t } = useTranslation()

  const navigate = useNavigate()

  const device = useGetDeviceByType(DEVICE_TYPE.TITO_PRINTER)

  const [date, setDate] = useState(new Date())

  const { data, isPending: queryIsPending } = useDailyMediaReport({ date })

  const { mutate: printDailyMediaReport, isPending: printIsPending } = usePrintDailyMediaReport()

  const deviceDisabled = device && device?.isActive ? false : true
  const printDisabled = deviceDisabled || printIsPending || queryIsPending

  const handlePrint = () => {
    printDailyMediaReport({
      date: date?.toISOString()
    })
  }

  useEffect(() => {
    setDate(new Date())
  }, [])

  return (
    <Container>
      <FullPageLoader loading={printIsPending || queryIsPending} />
      <div className="actions">
        <Button
          onClick={handlePrint}
          disabled={printDisabled}
          icon={(props) => <IconPrinter {...props} />}
        >
          {t('Print')}
        </Button>

        {deviceDisabled && (
          <Alert severity={severityMap.ERROR} text={t('Ticket printer is not available.')} />
        )}
      </div>

      <div className="full-width">
        <DateInput
          defaultValue={date}
          value={date}
          label={t('Date for report')}
          size="m"
          dateFormat="dd-MM-yyyy"
          onChange={(value) => setDate(value)}
        />
        <Divider height="0.25rem" />
      </div>

      <div className="section">
        <div className="title">{t('Bills')}</div>

        <div className="row">
          <DecimalInput
            size="m"
            disabled={true}
            label={t('Accepted count')}
            value={data?.billsAcceptedCount || 0}
          />
          <DecimalInput
            size="m"
            disabled={true}
            label={t('Accepted value')}
            value={data?.billsAcceptedValue || 0}
          />
        </div>

        <Divider height="0.25rem" />

        <div className="row">
          <DecimalInput
            size="m"
            disabled={true}
            label={t('Dispensed count')}
            value={data?.billsDispensedCount || 0}
          />
          <DecimalInput
            size="m"
            disabled={true}
            label={t('Dispensed value')}
            value={data?.billsDispensedValue || 0}
          />
        </div>

        <Divider height="0.25rem" />

        <div className="row">
          <DecimalInput
            size="m"
            disabled={true}
            label={t('Rejected by acceptor')}
            value={data?.billsRejectedByAcceptorCount || 0}
          />
          <DecimalInput
            size="m"
            disabled={true}
            label={t('Rejected by dispenser')}
            value={data?.billsRejectedByDispenserCount || 0}
          />
        </div>

        <Divider height="0.25rem" />
      </div>

      <div className="section">
        <div className="title">{t('Tickets')}</div>

        <div className="row">
          <DecimalInput
            size="m"
            disabled={true}
            label={t('Accepted count')}
            value={data?.ticketsAcceptedCount || 0}
          />
          <DecimalInput
            size="m"
            disabled={true}
            label={t('Accepted value')}
            value={data?.ticketsAcceptedValue || 0}
          />
        </div>

        <Divider height="0.25rem" />

        <div className="row">
          <DecimalInput
            size="m"
            disabled={true}
            label={t('Printed count')}
            value={data?.ticketsPrintedCount || 0}
          />
          <DecimalInput
            size="m"
            disabled={true}
            label={t('Printed value')}
            value={data?.ticketsPrintedValue || 0}
          />
        </div>

        <Divider height="0.25rem" />

        <div className="row full-width single-row">
          <DecimalInput
            size="m"
            disabled={true}
            label={t('Rejected by acceptor count')}
            value={data?.ticketsRejectedByAcceptorCount || 0}
          />
        </div>

        <Divider height="0.25rem" />
      </div>

      <div className="footer">
        <CircleButton
          icon={(props) => <IconLeftHalfArrow {...props} />}
          size="l"
          color="dark"
          textRight={t('Back')}
          onClick={() => navigate(-1)}
          shadow={true}
        />
      </div>
    </Container>
  )
}

export default DailyMedia
