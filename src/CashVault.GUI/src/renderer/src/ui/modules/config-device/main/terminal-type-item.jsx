/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import CheckboxInput from '@ui/components/inputs/checkbox-input'

const TerminalTypeItem = ({ options = [], selectedOptions = [], onChange = () => {} }) => {
  return (
    <div className="terminal-types-grid">
      {options.map((option) => (
        <CheckboxInput
          key={option.value}
          value={selectedOptions?.some((val) => val.value === option.value)}
          label={option.name}
          onChange={(e) => onChange(option.value, e.target.checked)}
        />
      ))}
    </div>
  )
}

export default TerminalTypeItem
