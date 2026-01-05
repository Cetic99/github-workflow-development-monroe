/* eslint-disable react/display-name */
/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import Modal from '@ui/components/modal'
import { forwardRef, useState } from 'react'
import styled from '@emotion/styled'
import Button from '@ui/components/button'
import { useTranslation } from '@domain/administration/stores'
import CheckboxInput from '@ui/components/inputs/checkbox-input'

const Container = styled.div`
  display: flex;
  padding-top: 2rem;
  flex-direction: column;
  gap: 2rem;
  height: 100%;

  & .actions {
    display: flex;
    gap: 1rem;
  }
`

const AddDeviceModal = forwardRef(({ onClose = () => {}, onAdd = () => {}, devices = [] }, ref) => {
  const { t } = useTranslation()
  const [selectedDevices, setSelectedDevices] = useState([])

  const handleSelect = (deviceName, checked) => {
    if (checked) {
      setSelectedDevices((prev) => (prev.includes(deviceName) ? prev : [...prev, deviceName]))
    } else {
      setSelectedDevices((prev) => prev.filter((d) => d !== deviceName))
    }
  }

  const DeviceItem = ({ option, selected, onSelect = () => {} }) => {
    return (
      <div>
        <CheckboxInput
          key={option}
          value={selected}
          label={t(option)}
          onChange={(e) => onSelect(option, e.target.checked)}
        />
      </div>
    )
  }

  return (
    <Modal ref={ref} onClose={onClose} title={`${t('Add device')}`}>
      <Container>
        {(devices || []).map((device) => (
          <DeviceItem
            key={device}
            option={device}
            selected={selectedDevices.some((x) => x === device)}
            onSelect={handleSelect}
          />
        ))}
        <div className="actions">
          <Button color="light" onClick={onClose}>
            {t('Cancel')}
          </Button>

          <Button onClick={() => onAdd(selectedDevices)}>{t('Add')}</Button>
        </div>
      </Container>
    </Modal>
  )
})
export default AddDeviceModal
