/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import Button from '@ui/components/button'
import IconToggleOff from '@ui/components/icons/IconToggleOff'
import IconToggleOn from '@ui/components/icons/IconToggleOn'
import IconRefresh from '@ui/components/icons/IconRefresh'
import IconInfoCircle from '@ui/components/icons/IconInfoCircle'
import IconExclamationMarkCircle from '@ui/components/icons/IconExclamationMarkCircle'
import { useTranslation } from '@domain/administration/stores'
import CheckboxInput from '@ui/components/inputs/checkbox-input'
import Modal from '@ui/components/modal'
import Alert from '@ui/components/alert'
import { useRef, useState } from 'react'
import { isNullOrEmptyOrSpaces } from '@domain/global/services'
import { useGetDeviceInfo } from '@domain/administration/queries'
import { useNavigate } from 'react-router-dom'
import IconViewBoard from '@ui/components/icons/IconViewBoard'
import { DEVICE_TYPE } from '@src/app/domain/device/constants'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  background: white;
  border-radius: 10px;
  padding: 1.5rem;

  & .md-header {
    display: flex;
    gap: 1rem;
    align-items: center;
  }

  & .md-name {
    font-weight: 600;
    font-size: 2rem;
    line-height: 2.25rem;
  }

  & .md-status {
    margin-left: auto;
    display: flex;
    align-items: center;
    gap: 0.25rem;

    font-weight: 600;
    font-size: 1.625rem;
    line-height: 2rem;
    color: var(--primary-dark);

    & span {
      color: ${(p) => (p.active ? 'var(--primary-dark)' : 'var(--error-dark)')};
    }
  }

  & .md-info {
    font-size: 1.5rem;
    line-height: 2rem;
    font-weight: 500;
    margin-top: 1rem;
  }

  & .md-desc {
    padding-top: 0.25rem;
    display: flex;
    gap: 0.25rem;
    color: var(--text-medium);
    font-weight: 600;
    font-size: 1rem;
    line-height: 1.125rem;
    text-transform: uppercase;

    & span:nth-of-type(2) {
      color: ${(p) => (p.active ? 'var(--text-medium)' : 'var(--error-dark)')};
      margin-left: auto;
    }
  }

  & .md-enabled {
    display: flex;
    gap: 1rem;
    align-items: center;
    margin-top: 1.5rem;
    padding: 1rem 0;
    border-top: 1px solid var(--border-light);
    border-bottom: 1px solid var(--border-light);

    font-weight: 500;
    font-size: 1.5rem;
    line-height: 1.75rem;
    color: ${(p) => (p.enabled ? 'black' : 'var(--text-medium)')};
  }

  & .md-connected {
    display: flex;
    gap: 1rem;
    align-items: center;
    padding: 1rem 0;
    border-bottom: 1px solid var(--border-light);

    font-weight: 500;
    font-size: 1.5rem;
    line-height: 1.75rem;
    color: ${(p) => (p.connected ? 'black' : 'var(--text-medium)')};
  }

  & .md-actions {
    display: flex;
    gap: 1rem;
    align-items: center;
    padding-top: 1.5rem;
    overflow-y: auto;
    scrollbar-width: none;

    & .action-button {
      margin-left: auto;
    }
  }

  & .mr-left {
    margin-left: auto;
  }
`

const ModalContent = styled.div`
  & .modal-header {
    font-weight: 600;
    font-size: 1.75rem;
    line-height: 1.875rem;
    margin-bottom: 1rem;
    white-space: pre-wrap;
    word-break: break-word;
  }

  & .modal-content {
    white-space: pre-wrap;
    word-break: break-word;
  }
`

const ManageDevice = ({
  name = 'Bill Dispenser',
  desc = 'CashVault.BillDispenserDriver.JCM.F53',
  type = 'billdispenser',
  active = false,
  status = 'OK',
  error,
  warning,
  enabled = false,
  connected = true,
  onEnableDisable = () => {},
  onReset = () => {}
}) => {
  const { t } = useTranslation()
  const modalRef = useRef()
  const [infoOpen, setInfoOpen] = useState(false)
  const navigate = useNavigate()
  const infoQuery = useGetDeviceInfo({ type, enabled: infoOpen })

  const getName = () => {
    if (isNullOrEmptyOrSpaces(name)) return name

    if (type === DEVICE_TYPE.BILL_DISPENSER) return 'Bill dispenser'
    if (type === DEVICE_TYPE.TITO_PRINTER) return 'Ticket printer'
    if (type === DEVICE_TYPE.BILL_ACCEPTOR) return 'Bill acceptor'
    if (type === DEVICE_TYPE.CARD_READER) return 'Card reader'
    if (type === DEVICE_TYPE.COIN_ACCEPTOR) return 'Coin acceptor'

    return t(type)
  }

  const handleOnShowMoreInfo = () => {
    setInfoOpen(true)
    modalRef?.current?.showModal()
  }

  const handleDeviceClick = () => {
    navigate(`/device-diagnostic/${type}`)
  }

  return (
    <Container active={active} enabled={enabled} connected={connected}>
      <div className="md-header">
        <span className="md-name">{getName()}</span>

        <div className="md-status">
          {active && <CheckboxInput size="s" value={active} />}
          {!active && <IconExclamationMarkCircle size="m" color="var(--error-dark)" />}

          <span>
            {active && t('Active')}
            {!active && t('Inactive')}
          </span>
        </div>
      </div>

      <div className="md-desc">
        <span>{desc}</span>
        <span>{t('Status')}</span>
      </div>

      {!isNullOrEmptyOrSpaces(warning) && (
        <div className="md-info">
          <Alert text={warning} severity="warning" iconSize="m" />
        </div>
      )}

      {!isNullOrEmptyOrSpaces(error) && (
        <div className="md-info">
          <Alert text={error} severity="error" iconSize="m" />
        </div>
      )}

      <div className="md-enabled">
        <span>{t('Enabled')}</span>

        <CheckboxInput size="s" value={enabled} className="mr-left" />
      </div>

      <div className="md-connected">
        <span>{t('Connected')}</span>

        <CheckboxInput size="s" value={connected} className="mr-left" />
      </div>

      <div className="md-actions">
        <Button
          icon={(props) => <IconInfoCircle {...props} />}
          onClick={() => handleOnShowMoreInfo()}
        >
          {t('More info')}
        </Button>

        <Button icon={(props) => <IconViewBoard {...props} />} onClick={() => handleDeviceClick()}>
          {t('Inspect')}
        </Button>

        <Button
          color={enabled ? 'dark' : 'light'}
          className="action-button"
          icon={(props) => {
            if (enabled) return <IconToggleOn {...props} />
            if (!enabled) return <IconToggleOff {...props} />
          }}
          onClick={() => onEnableDisable(enabled, type)}
        >
          {enabled && t('Enabled')}
          {!enabled && t('Disabled')}
        </Button>

        <Button onClick={() => onReset(type)} icon={(props) => <IconRefresh {...props} />}>
          {t('Reset')}
        </Button>
      </div>

      <Modal ref={modalRef} size="m" onClose={() => modalRef?.current?.close()}>
        <ModalContent>
          <div className="modal-header">{t('Additional info')}</div>

          {isNullOrEmptyOrSpaces(infoQuery?.data?.info) ? (
            <div>{t('No data')}</div>
          ) : (
            <div className="modal-content">{infoQuery?.data?.info}</div>
          )}
        </ModalContent>
      </Modal>
    </Container>
  )
}

export default ManageDevice
