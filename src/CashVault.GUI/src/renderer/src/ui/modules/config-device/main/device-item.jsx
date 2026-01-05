/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import DropdownInput from '@ui/components/inputs/dropdown-input'
import Button from '@ui/components/button'
import IconTrashCan from '@ui/components/icons/IconTrashCan'

const DeviceItem = ({
  name,
  label,
  value,
  options,
  interfaces,
  interfaceValue,
  onDeviceDriverChange = () => {},
  onInterfaceChange = () => {},
  onDeviceRemove = () => {}
}) => {
  return (
    <div className="device-item">
      <div className="device-item-dropdown">
        <DropdownInput
          label={label}
          options={options}
          value={value}
          onChange={(option) => onDeviceDriverChange(name, option?.value)}
        />
      </div>
      <div className="device-item-dropdown">
        <DropdownInput
          label="Interface"
          options={interfaces}
          value={interfaceValue}
          onChange={(option) => onInterfaceChange(name, option?.value)}
        />
      </div>
      <div>
        <div className="dummy-div"></div>
        <Button
          color="transparent"
          icon={(props) => <IconTrashCan {...props} />}
          onClick={() => onDeviceRemove(name)}
          confirmAction={true}
        ></Button>
      </div>
    </div>
  )
}

export default DeviceItem
