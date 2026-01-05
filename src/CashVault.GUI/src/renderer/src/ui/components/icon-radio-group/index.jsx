/* eslint-disable prettier/prettier */
/* eslint-disable react/prop-types */

import styled from '@emotion/styled'
import IconCheckmark from '@ui/components/icons/IconCheckmark'
import { useTranslation } from '@domain/administration/stores'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  background-color: var(--bg-light);
  color: var(--text-dark);
  border-radius: var(--border-radius);
`

const Option = styled.div`
  ${({ isLast }) => (isLast ? '' : 'border-bottom: 1px solid var(--border-dark);')}

  display: flex;
  justify-content: space-between;
  align-items: center;

  .icon-radio-label {
    display: flex;
    gap: 1rem;
    padding: 1rem 2rem;
    align-items: center;
    color: ${({ isActive }) => (isActive ? 'var(--text-dark)' : 'var(--text-inactive-dark)')};
    font-weight: ${({ isActive }) => (isActive ? '500' : 'normal')};
    cursor: pointer;
    transition: color 0.2s ease;

    font-size: 1.5rem;
  }
`

const IconRadioGroup = ({ options, value, onChange }) => {
  const { t } = useTranslation()
  return (
    <Container className="icon-radio-group">
      {options.map((option, idx) => (
        <Option
          className="option"
          key={option.value}
          isLast={idx === options.length - 1}
          onClick={() => onChange(option.value)}
          isActive={option.value === value}
        >
          <div className="icon-radio-label">
            {option?.icon && <div className={`text-icon`}>{option.icon}</div>}
            <span className="label-text">{t(option?.label)}</span>
          </div>
          {option.value === value && (
            <div>
              <IconCheckmark color="white" />
            </div>
          )}
        </Option>
      ))}
    </Container>
  )
}

export default IconRadioGroup
