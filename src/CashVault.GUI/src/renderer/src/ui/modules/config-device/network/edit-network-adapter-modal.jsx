/* eslint-disable react/display-name */
/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import Modal from '@ui/components/modal'
import { forwardRef, useEffect, useState } from 'react'
import styled from '@emotion/styled'
import Button from '@ui/components/button'
import TextInput from '@ui/components/inputs/text-input'
import CheckBoxInput from '@ui/components/inputs/checkbox-input'
import { useTranslation } from '@domain/administration/stores'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  gap: 2rem;
  height: 100%;

  & .header {
    font-weight: 600;
    font-size: 1.75rem;
    line-height: 1.875rem;
    margin-bottom: 1rem;
  }

  .content {
    display: flex;
    flex-direction: column;
    gap: 1rem;

    .label {
      font-weight: 600;
      font-size: 1.125rem;
      line-height: 1.25rem;
      color: var(--text-dark);
    }
  }

  & .actions {
    display: flex;
    gap: 1rem;
  }
`

const InputRow = styled.div`
  display: flex;
  align-items: center;
  gap: 1rem;
`

const Separator = styled.div`
  width: 100%;
  height: 2px;
  background-color: var(--border-light);
  margin: 1rem 0;
`

const EditNetworkAdapterModal = forwardRef(
  ({ adapter, disabled, onClose = () => {}, onSave = () => {} }, ref) => {
    const { t } = useTranslation()
    const [networkAdapterSettings, setNetworkAdapterSettings] = useState({})

    useEffect(() => {
      if (adapter) {
        setNetworkAdapterSettings({
          ...adapter,
          ...adapter?.networkAdapterInfo
        })
      }
    }, [adapter])

    const handleDhcpEnabledChange = (value) => {
      if (value === true) {
        setNetworkAdapterSettings({
          ...networkAdapterSettings,
          isDhcpEnabled: true,
          ipAddress: '',
          networkMask: '',
          gateway: ''
        })
      } else {
        setNetworkAdapterSettings({ ...networkAdapterSettings, isDhcpEnabled: false })
      }
    }

    const handleDnsEnabledChange = (value) => {
      if (value === true) {
        setNetworkAdapterSettings({
          ...networkAdapterSettings,
          isDnsEnabled: true,
          preferredDns: '',
          alternateDns: ''
        })
      } else {
        setNetworkAdapterSettings({
          ...networkAdapterSettings,
          isDnsEnabled: false,
          preferredDns: '',
          alternateDns: ''
        })
      }
    }

    const handleOnSave = () => {
      onSave({
        name: networkAdapterSettings?.name,
        isDhcpEnabled: networkAdapterSettings?.isDhcpEnabled,
        ipAddress: networkAdapterSettings?.ipAddress,
        networkMask: networkAdapterSettings?.networkMask,
        gateway: networkAdapterSettings?.gateway,
        isDnsEnabled: networkAdapterSettings?.isDnsEnabled,
        preferredDns: networkAdapterSettings?.preferredDns,
        alternateDns: networkAdapterSettings?.alternateDns
      })

      ref?.current?.close()
    }

    //===============================================

    const render = () => {
      return (
        <div className="content">
          <InputRow>
            <CheckBoxInput
              name="dhcpEnabled"
              value={networkAdapterSettings.isDhcpEnabled ?? false}
              onChange={(e) => handleDhcpEnabledChange(e.target.checked)}
            />
            <label className="label">DHCP Enabled</label>
          </InputRow>
          <TextInput
            label="IP Address"
            disabled={networkAdapterSettings?.isDhcpEnabled || disabled}
            value={networkAdapterSettings?.ipAddress ?? ''}
            onChange={(e) =>
              setNetworkAdapterSettings({ ...networkAdapterSettings, ipAddress: e.target.value })
            }
          />
          <TextInput
            label="Network mask"
            disabled={networkAdapterSettings?.isDhcpEnabled || disabled}
            value={networkAdapterSettings?.networkMask ?? ''}
            onChange={(e) =>
              setNetworkAdapterSettings({ ...networkAdapterSettings, networkMask: e.target.value })
            }
          />
          <TextInput
            label="Gateway"
            name="gateway"
            disabled={networkAdapterSettings?.isDhcpEnabled || disabled}
            value={networkAdapterSettings?.gateway ?? ''}
            onChange={(e) =>
              setNetworkAdapterSettings({ ...networkAdapterSettings, gateway: e.target.value })
            }
          />
          <Separator />
          <InputRow>
            <CheckBoxInput
              name="dnsEnabled"
              value={networkAdapterSettings.isDnsEnabled ?? false}
              onChange={(e) => handleDnsEnabledChange(e.target.checked)}
            />
            <label className="label">Obtain DNS server address automatically</label>
          </InputRow>
          <TextInput
            label="IP Address"
            disabled={networkAdapterSettings?.isDnsEnabled || disabled}
            value={networkAdapterSettings?.preferredDns ?? ''}
            onChange={(e) =>
              setNetworkAdapterSettings({ ...networkAdapterSettings, preferredDns: e.target.value })
            }
          />
          <TextInput
            label="Alternate DNS server"
            disabled={networkAdapterSettings?.isDnsEnabled || disabled}
            value={networkAdapterSettings?.alternateDns ?? ''}
            onChange={(e) =>
              setNetworkAdapterSettings({ ...networkAdapterSettings, alternateDns: e.target.value })
            }
          />
        </div>
      )
    }

    return (
      <Modal
        ref={ref}
        onClose={onClose}
        title={`${t('Network settings')} - ${networkAdapterSettings?.name}`}
      >
        <Container>
          {render()}
          <div className="actions">
            <Button color="light" onClick={onClose}>
              Cancel
            </Button>

            <Button confirmAction={true} onClick={handleOnSave}>
              Save
            </Button>
          </div>
        </Container>
      </Modal>
    )
  }
)

export default EditNetworkAdapterModal
