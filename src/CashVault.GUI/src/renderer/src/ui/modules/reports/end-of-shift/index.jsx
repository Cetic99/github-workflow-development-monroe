/* eslint-disable prettier/prettier */

import styled from '@emotion/styled'
import { useTranslation } from '@domain/administration/stores'
import { DEVICE_TYPE } from '@domain/device/constants'
import { useGetDeviceByType } from '@domain/device/stores'
import { useNavigate } from 'react-router-dom'
import { useEndOfShiftReport } from '@domain/reports/queries'
import { useHarvestShiftMoney } from '@domain/operator/commands'
import { usePrintEndOfShiftReport } from '@domain/reports/commands'

import IconPrinter from '@ui/components/icons/IconPrinter'
import IconLeftHalfArrow from '@ui/components/icons/IconLeftHalfArrow'

import Button from '@ui/components/button'
import TextInput from '@ui/components/inputs/text-input'
import Divider from '@ui/components/divider'
import CircleButton from '@ui/components/circle-button'
import DecimalInput from '@ui/components/inputs/decimal-input'
import Modal from '@ui/components/modal'
import Alert from '@ui/components/alert'
import { severityMap } from '@ui/components/alert/severity'
import FullPageLoader from '@ui/components/full-page-loader'

import { useEffect, useRef, useState } from 'react'
import moment from 'moment'

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

  & .row {
    display: flex;
    gap: 1rem;

    > div {
      width: 100%;
    }
  }

  & .single-row {
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

const ModalContent = styled.div`
  & .modal-header {
    font-weight: 600;
    font-size: 1.75rem;
    line-height: 1.875rem;
    margin-bottom: 1rem;
  }

  & .modal-content {
    padding-top: 2rem;
    display: flex;
    flex-direction: column;
    gap: 0.875rem;

    font-family: Poppins;
    font-weight: 400;
    font-size: 1.75rem;
    line-height: 2.5rem;
    letter-spacing: -0.001rem;
  }

  & .modal-footer {
    width: 100%;
    display: flex;
    justify-content: space-between;
    padding-top: 2rem;

    & .print-group {
      display: flex;
      gap: 1.5rem;
      margin-left: 0.5rem;
    }
  }
`

const EndOfShift = () => {
  const { t } = useTranslation()

  const navigate = useNavigate()

  const device = useGetDeviceByType(DEVICE_TYPE.TITO_PRINTER)

  const [formData, setFormData] = useState({})

  const printModalRef = useRef()

  const { data, isPending, refetch } = useEndOfShiftReport({
    enabled: true
  })

  const { mutate: harvestShiftMoney, isPending: harvestShiftMoneyIsPending } = useHarvestShiftMoney(
    () => {
      refetch()
      printModalRef?.current?.close()
    }
  )

  const { mutate: printEndOfShiftReport, isPending: reportPending } = usePrintEndOfShiftReport()

  useEffect(() => {
    setFormData(data)
  }, [data])

  const deviceDisabled = device && device?.isActive ? false : true
  const printDisabled = deviceDisabled || reportPending || isPending || harvestShiftMoneyIsPending

  const onInputChange = (accessor, value) => {
    setFormData((prev) => {
      return {
        ...prev,
        [accessor]: value
      }
    })
  }

  const handlePrint = () => {
    printEndOfShiftReport()
    printModalRef?.current?.close()
  }

  const onPrintAndClearTotals = () => {
    harvestShiftMoney()
    printModalRef?.current?.close()
  }

  const getFormattedDate = (date) => {
    if (!date) {
      return '/'
    }

    const currentDate = moment(date)
    const formatedValue = currentDate.isValid() ? currentDate.format('MM-DD-YYYY') : '/'

    return formatedValue
  }

  return (
    <Container>
      <FullPageLoader loading={reportPending || harvestShiftMoneyIsPending} />
      <div className="actions">
        <Button
          onClick={() => printModalRef?.current?.showModal()}
          icon={(props) => <IconPrinter {...props} />}
          disabled={printDisabled}
        >
          {t('Print')}
        </Button>

        {deviceDisabled && (
          <Alert severity={severityMap.ERROR} text={t('Ticket printer is not available.')} />
        )}
      </div>

      <div className="full-width">
        <div className="row full-width">
          <TextInput
            size="m"
            label={t('Date of last report')}
            value={getFormattedDate(data?.fromDate ? new Date(data?.fromDate) : null)}
            disabled={true}
            className="full-width"
          />
          <TextInput
            size="m"
            label={t('Current date')}
            value={getFormattedDate(data?.toDate ? new Date(data?.toDate) : null)}
            disabled={true}
            className="full-width"
          />
        </div>
        <Divider height="0.25rem" />
      </div>

      <div className="section">
        <div className="title">{t('Bills')}</div>

        <div className="row">
          <DecimalInput
            size="m"
            disabled={true}
            label={t('Accepted count')}
            value={formData?.billsAcceptedCount || 0}
            onValueChange={(value) => {
              onInputChange('billsAcceptedCount', value)
            }}
            onClear={() => {
              onInputChange('billsAcceptedCount', 0)
            }}
          />
          <DecimalInput
            size="m"
            disabled={true}
            label={t('Accepted value')}
            value={formData?.billsAcceptedValue || 0}
            onValueChange={(value) => {
              onInputChange('billsAcceptedValue', value)
            }}
            onClear={() => {
              onInputChange('billsAcceptedValue', 0)
            }}
          />
        </div>

        <Divider height="0.25rem" />

        <div className="row">
          <DecimalInput
            size="m"
            disabled={true}
            label={t('Dispensed count')}
            value={formData?.billsDispensedCount || 0}
            onValueChange={(value) => {
              onInputChange('billsDispensedCount', value)
            }}
            onClear={() => {
              onInputChange('billsDispensedCount', 0)
            }}
          />
          <DecimalInput
            size="m"
            disabled={true}
            label={t('Dispensed value')}
            value={formData?.billsDispensedValue || 0}
            onValueChange={(value) => {
              onInputChange('billsDispensedValue', value)
            }}
            onClear={() => {
              onInputChange('billsDispensedValue', 0)
            }}
          />
        </div>

        <Divider height="0.25rem" />

        <div className="row">
          <DecimalInput
            size="m"
            disabled={true}
            label={t('Rejected by acceptor')}
            value={formData?.billsRejectedByAcceptorCount || 0}
            onValueChange={(value) => {
              onInputChange('billsRejectedByAcceptorCount', value)
            }}
            onClear={() => {
              onInputChange('billsRejectedByAcceptorCount', 0)
            }}
          />
          <DecimalInput
            size="m"
            disabled={true}
            label={t('Rejected by dispenser')}
            value={formData?.billsRejectedByDispenserCount || 0}
            onValueChange={(value) => {
              onInputChange('billsRejectedByDispenserCount', value)
            }}
            onClear={() => {
              onInputChange('billsRejectedByDispenserCount', 0)
            }}
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
            value={formData?.ticketsAcceptedCount || 0}
            onValueChange={(value) => {
              onInputChange('ticketsAcceptedCount', value)
            }}
            onClear={() => {
              onInputChange('ticketsAcceptedCount', 0)
            }}
          />
          <DecimalInput
            size="m"
            disabled={true}
            label={t('Accepted value')}
            value={formData?.ticketsAcceptedValue || 0}
            onValueChange={(value) => {
              onInputChange('ticketsAcceptedValue', value)
            }}
            onClear={() => {
              onInputChange('ticketsAcceptedValue', 0)
            }}
          />
        </div>

        <Divider height="0.25rem" />

        <div className="row">
          <DecimalInput
            size="m"
            disabled={true}
            label={t('Printed count')}
            value={formData?.ticketsPrintedCount || 0}
            onValueChange={(value) => {
              onInputChange('ticketsPrintedCount', value)
            }}
            onClear={() => {
              onInputChange('ticketsPrintedCount', 0)
            }}
          />
          <DecimalInput
            size="m"
            disabled={true}
            label={t('Printed value')}
            value={formData?.ticketsPrintedValue || 0}
            onValueChange={(value) => {
              onInputChange('ticketsPrintedValue', value)
            }}
            onClear={() => {
              onInputChange('ticketsPrintedValue', 0)
            }}
          />
        </div>

        <Divider height="0.25rem" />

        <div className="row full-width single-row">
          <DecimalInput
            size="m"
            disabled={true}
            label={t('Rejected by acceptor count')}
            value={formData?.ticketsRejectedByAcceptorCount || 0}
            onValueChange={(value) => {
              onInputChange('ticketsRejectedByAcceptorCount', value)
            }}
            onClear={() => {
              onInputChange('ticketsRejectedByAcceptorCount', 0)
            }}
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

      {/* Print modal */}
      <Modal ref={printModalRef} onClose={() => printModalRef?.current?.close()}>
        <ModalContent>
          <div className="modal-header">{t('Print report')}</div>
          <div className="modal-content">
            {t(
              'You can choose to print the report as-is or print and reset the totals after printing.'
            )}
          </div>

          <div className="modal-footer">
            <Button
              className="action-btn"
              color="light"
              onClick={() => printModalRef?.current?.close()}
            >
              {t('Cancel')}
            </Button>
            <div className="print-group">
              <Button onClick={handlePrint} icon={(props) => <IconPrinter {...props} />}>
                {t('Print')}
              </Button>
              <Button onClick={onPrintAndClearTotals} icon={(props) => <IconPrinter {...props} />}>
                {t('Print and clear totals')}
              </Button>
            </div>
          </div>
        </ModalContent>
      </Modal>
    </Container>
  )
}

export default EndOfShift
