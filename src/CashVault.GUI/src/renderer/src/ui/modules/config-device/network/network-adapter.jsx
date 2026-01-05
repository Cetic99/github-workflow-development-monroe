/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import Button from '@ui/components/button'
import IconToggleOff from '@ui/components/icons/IconToggleOff'
import IconExclamationMarkCircle from '@ui/components/icons/IconExclamationMarkCircle'
import { useTranslation } from '@domain/administration/stores'
import CheckboxInput from '@ui/components/inputs/checkbox-input'
import { useRef } from 'react'
import EditNetworkAdapterModal from './edit-network-adapter-modal'

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
    color: black;
  }

  & .md-actions {
    display: flex;
    gap: 1rem;
    align-items: center;
    padding-top: 1.5rem;

    & .action-button {
      margin-left: auto;
    }
  }
`

const NetworkAdapter = ({ adapter, disabled, onSave }) => {
  const { t } = useTranslation()
  const modalRef = useRef()
  const active = adapter.status != 'Down'

  const handleOnEdit = () => {
    modalRef?.current?.showModal()
  }

  return (
    <Container active={active}>
      <div className="md-header">
        <span className="md-name">{adapter?.name}</span>

        <div className="md-status">
          {active && <CheckboxInput size="s" value={active} />}
          {!active && <IconExclamationMarkCircle size="m" color="var(--error-dark)" />}

          <span>
            {active && t('Up')}
            {!active && t('Down')}
          </span>
        </div>
      </div>

      <div className="md-desc">
        <span>{adapter?.description}</span>
        <span>{t('Status')}</span>
      </div>

      <div className="md-enabled">Mac address: {adapter?.macAddress || t('Not available')}</div>

      <div className="md-actions">
        <Button
          disabled={disabled}
          icon={(props) => <IconToggleOff {...props} />}
          onClick={() => handleOnEdit()}
        >
          {t('Edit')}
        </Button>
      </div>
      <EditNetworkAdapterModal
        ref={modalRef}
        adapter={adapter}
        disabled={disabled}
        onClose={() => modalRef.current.close()}
        onSave={onSave}
      />
    </Container>
  )
}

export default NetworkAdapter
