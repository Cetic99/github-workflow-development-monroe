/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import {
  getTerminalTypeConfiguration,
  useConfigurationDeviceMain
} from '@domain/configuration/queries/device'
import { useSaveDeviceMainData } from '@domain/configuration/mutations/device'
import FullPageLoader from '@ui/components/full-page-loader'
import styled from '@emotion/styled'
import { useEffect, useRef, useState } from 'react'
import CircleButton from '@ui/components/circle-button'
import IconRightHalfArrow from '@ui/components/icons/IconRightHalfArrow'
import { useTranslation } from '@domain/administration/stores'
import IconLeftHalfArrow from '@ui/components/icons/IconLeftHalfArrow'
import { useNavigate } from 'react-router-dom'
import Button from '@ui/components/button'
import IconPlus from '@ui/components/icons/IconPlus'
import { getDevicesByTerminalTypes, setTerminalTypeConfig } from '@src/app/domain/global/stores'
import AddDeviceModal from './add-device-modal'
import DeviceItem from './device-item'
import TerminalTypeItem from './terminal-type-item'

const Container = styled.div`
  font-weight: 600;
  height: 100%;

  display: flex;
  flex-direction: column;
  justify-content: space-between;

  & .title {
    font-size: 1.75rem;
    margin-top: 2rem;
    margin-bottom: 1rem;
  }

  .device-intefaces-header {
    margin-top: 4rem;
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  & .terminal-main-form {
    height: 100%;
  }

  & .device-item {
    display: flex;
    gap: 2rem;
    padding: 1rem 0;
    border-bottom: 1px solid var(--primary-medium);

    & .device-item-dropdown {
      width: 50%;
    }

    & .dummy-div {
      height: 2rem;
    }
  }

  & .config-main-footer {
    padding-top: 3rem;
    margin-top: auto;
    display: flex;
    gap: 4rem;
    justify-content: space-between;
    position: sticky;
    bottom: 0;
    z-index: 10;

    pointer-events: none;

    & > * {
      pointer-events: all;
    }
  }

  .terminal-types-grid {
    display: grid;
    grid-template-columns: 1fr 1fr 1fr;
    gap: 2rem;
    .label {
      font-size: 1.625rem;
    }
    padding-bottom: 1rem;
    border-bottom: 1px solid var(--primary-medium);
  }

  & .content {
    overflow: visible;
  }

  & .no-data-label {
    text-align: center;
    padding: 2rem;
    color: var(--text-inactive-dark);
  }
`

const ConfigDeviceMain = () => {
  const { data: mainConfigData, isLoading } = useConfigurationDeviceMain()
  const [mainConfig, setMainConfig] = useState({})
  const { t } = useTranslation()
  const formRef = useRef()
  const modalRef = useRef()
  const navigate = useNavigate()

  useEffect(() => {
    setMainConfig(mainConfigData)
  }, [mainConfigData])

  const onSuccessSave = async () => {
    const terminalTypeConf = await getTerminalTypeConfiguration()

    setTerminalTypeConfig(terminalTypeConf)
  }

  const { mutate: saveMainConfiguration, isPending } = useSaveDeviceMainData(onSuccessSave)

  const terminalType = mainConfig?.items?.filter((item) => item.name === 'terminalType')?.[0]

  const handleOnInterfaceChange = (name, value) => {
    const updatedItems = mainConfig.items.map((item) => {
      if (item.name === name) {
        return { ...item, interface: { ...item.interface, value } }
      }
      return item
    })
    setMainConfig({ ...mainConfig, items: updatedItems })
  }

  const handleDeviceDriverChange = (name, value) => {
    const updatedItems = mainConfig.items.map((item) => {
      if (item.name === name) {
        return { ...item, value }
      }
      return item
    })
    setMainConfig({ ...mainConfig, items: updatedItems })
  }

  const handleDeviceRemove = (name) => {
    const updatedItems = mainConfig.items.filter((item) => item.name !== name)
    setMainConfig({ ...mainConfig, items: updatedItems })
  }
  const handleTerminalTypeChange = (value, checked) => {
    let updatedTerminalTypes = [...(mainConfig.terminalTypes || [])]

    if (checked) {
      updatedTerminalTypes.push({
        name: value,
        value: value
      })
    } else {
      updatedTerminalTypes = updatedTerminalTypes.filter((type) => type.value !== value)
    }

    const supportedDevices =
      getDevicesByTerminalTypes(updatedTerminalTypes.map((x) => x.value)) || []

    const filteredItems = mainConfig.items.filter((item) => supportedDevices.includes(item.name))

    setMainConfig({
      ...mainConfig,
      items: filteredItems,
      terminalTypes: updatedTerminalTypes
    })
  }

  const handleAddDevice = (selectedDevices) => {
    const existingDeviceNames = mainConfig.items.map((item) => item.name)
    const newDevices = selectedDevices.filter((device) => !existingDeviceNames.includes(device))
    const newDeviceItems = newDevices.map((device) => ({
      name: device,
      label: `${t(device)} TYPE`,
      value: '',
      interface: { value: '', options: [] }
    }))
    setMainConfig({ ...mainConfig, items: [...mainConfig.items, ...newDeviceItems] })
    modalRef.current.close()
  }

  const handleSubmit = (e) => {
    e.preventDefault()
    let data = { deviceName: mainConfig?.deviceName }
    data.items = mainConfig?.items?.map((item) => {
      return {
        name: item.name,
        value: item.value,
        interface: item.interface?.value || null
      }
    })

    data.terminalTypes = (mainConfig?.terminalTypes || [])?.map((type) => type.value) || []

    saveMainConfiguration(data)
  }

  const onGoBack = () => {
    navigate('/')
  }

  const handleOpenModal = (e) => {
    e?.preventDefault()
    modalRef?.current?.showModal()
  }

  return (
    <Container className="config-device-main">
      <FullPageLoader loading={isLoading || isPending} />

      <div id="terminal-main" className="terminal-main-form">
        <div className="content">
          {/* <TextInput
            label={t('Device name')}
            value={mainConfig?.deviceName || ''}
            onChange={handleDeviceNameChange}
            size="m"
          /> */}
          <TerminalTypeItem
            options={mainConfig?.terminalTypeOptions || []}
            selectedOptions={mainConfig?.terminalTypes || []}
            value={terminalType?.terminalTypes}
            onChange={handleTerminalTypeChange}
          />
          <div className="device-intefaces-header">
            <div className="title">{t('Devices and interfaces')}</div>
            <Button
              icon={(props) => <IconPlus {...props} />}
              disabled={mainConfig?.terminalTypes?.length === 0}
              onClick={(e) => handleOpenModal(e)}
            >
              {t('Add device')}
            </Button>
          </div>
          {mainConfig?.items
            ?.filter((item) => item.name !== 'terminalType')
            .map((item, index) => {
              return (
                <DeviceItem
                  key={`${item.name}-${index}`}
                  name={item.name}
                  type={item.name}
                  label={item.label}
                  value={item.value}
                  options={mainConfig?.availableDeviceModels?.[item.name] || []}
                  interfaces={mainConfig?.availablePorts || []}
                  interfaceValue={item.interface?.value || ''}
                  onInterfaceChange={handleOnInterfaceChange}
                  onDeviceDriverChange={handleDeviceDriverChange}
                  onDeviceRemove={handleDeviceRemove}
                />
              )
            })}
          {mainConfig?.items?.length === 0 && (
            <div className="no-data-label">{t('No added devices')}</div>
          )}
        </div>
      </div>
      <div className="config-main-footer">
        <CircleButton
          icon={(props) => <IconLeftHalfArrow {...props} />}
          size="l"
          color="dark"
          textRight={t('Back')}
          onClick={onGoBack}
          shadow={true}
        />

        <CircleButton
          icon={(props) => <IconRightHalfArrow {...props} />}
          size="l"
          color="dark"
          textRight={t('Accept')}
          onClick={(e) => handleSubmit(e)}
          shadow={true}
        />
      </div>
      <AddDeviceModal
        ref={modalRef}
        devices={
          getDevicesByTerminalTypes((mainConfig?.terminalTypes || [])?.map((x) => x.value) || []) ||
          []
        }
        onClose={() => modalRef.current.close()}
        onAdd={handleAddDevice}
      />
    </Container>
  )
}
export default ConfigDeviceMain
