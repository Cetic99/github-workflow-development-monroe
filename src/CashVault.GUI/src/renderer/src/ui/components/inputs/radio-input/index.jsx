/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
/* eslint-disable no-unused-vars */

import styled from '@emotion/styled'

const Container = styled.div`
  .radio {
    display: flex;
    gap: 0.5rem;
    align-items: center;
    font-weight: 500;
    cursor: pointer;
    font-size: ${(p) => (p.size == 's' ? '1.25rem' : '1.625rem')};
  }

  .radio input {
    display: none;
  }

  .checkmark {
    width: 2rem;
    height: 2rem;
    border: 2px solid var(--text-medium);
    border-radius: 50%;
    display: inline-block;
    position: relative;
    box-sizing: border-box;
  }

  .radio input:checked + .checkmark {
    border-color: var(--primary-dark);
  }

  .radio input:checked + .checkmark::after {
    content: '';
    width: 1.25rem;
    height: 1.25rem;
    background: var(--primary-dark);
    border-radius: 50%;
    position: absolute;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
  }

  .radio.disabled {
    opacity: 0.6;
    cursor: not-allowed;
  }

  .info {
    margin-left: 0.5rem;
    display: inline-flex;
    cursor: pointer;
  }
`

const RadioItemInput = ({
  value = true,
  label,
  onChange = () => {},
  className,
  disabled = false,
  size = 's',
  groupName
}) => {
  return (
    <>
      <Container className={className} size={size}>
        <label className={`radio ${disabled ? 'disabled' : ''}`}>
          <input
            type="radio"
            name={groupName}
            value={label}
            checked={value}
            disabled={disabled}
            onChange={(e) => {
              if (disabled === true) return

              onChange(e)
            }}
          />
          <span className="checkmark" />
          {label}
        </label>
      </Container>
    </>
  )
}

export default RadioItemInput
