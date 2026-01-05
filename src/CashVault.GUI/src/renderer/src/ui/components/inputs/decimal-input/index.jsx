/* eslint-disable prettier/prettier */
/* eslint-disable react/prop-types */
/* eslint-disable react/display-name */

import CurrencyInput from 'react-currency-input-field'
import styled from '@emotion/styled'
import IconCircleRemove from '@ui/components/icons/IconCircleRemove'
import { forwardRef } from 'react'
import { useDecimalsConfig } from '@domain/administration/stores/regional'
import { DefaultDecimalConfig } from '@domain/administration/constants'

const Container = styled.div`
  background-color: var(--bg-dark);
  border-radius: var(--border-radius-input);
  display: flex;
  gap: 1rem;
  align-items: center;
  padding: 0 1.25rem;

  & .separator {
    width: 1.5px;
    height: 2.875rem;
    background-color: var(--primary-medium);
  }

  & .sufix {
    font-weight: 500;
    font-size: 1.875rem;
    line-height: 2.375rem;
    text-align: right;
    color: black;
  }

  & .disabled {
    color: var(--text-default);
  }

  & .decimal-input {
    outline: none;
    border: none;
    background-color: var(--bg-dark);
    width: 100%;
    height: 100%;
    font-size: 4.375rem;
    font-weight: 700;
    line-height: 4.625rem;
    padding-top: 0.813rem;
    padding-bottom: 0.813rem;
    border-radius: var(--border-radius-input);
    text-align: ${(p) => (p.valuePosition === 'right' ? 'right' : 'left')};

    ${(p) => {
      if (p.size === 'm') {
        return `
          font-weight: 600;
          font-size: 2.125rem;
          line-height: 2.375rem;
          letter-spacing: -3%;
          padding-top: 0.713rem;
          padding-bottom: 0.713rem;  
        `
      }
    }}

    &:active {
      outline: none;
    }
  }
`

const Label = styled.div`
  font-weight: 600;
  font-size: 1.125rem;
  line-height: 1.375rem;
  color: var(--text-default);
  text-transform: uppercase;
  margin-bottom: 0.5rem;
`

const DecimalInput = forwardRef(
  (
    {
      size = 'l',
      sufix,
      decimalsLimit,
      defaultValue,
      max = 9999999,
      value,
      allowNegativeValue = false,
      disabled = false,
      onChange = () => {},
      onValueChange = () => {},
      onClear = () => {},
      inputType = 'text',
      props = {},
      allowDecimals = true,
      label,
      valuePosition = 'right',
      hasClearButton = true,
      selectOnFocus = true,
      name
    },
    ref
  ) => {
    const config = useDecimalsConfig()

    const handleOnClear = (e) => {
      e?.stopPropagation()

      if (disabled == true) return

      onClear(e)
    }

    const handleOnValueChange = (value, name, values) => {
      if (disabled) return

      if (value > max) {
        value = max
      }

      onValueChange(value, name, values)
    }

    return (
      <div>
        {label && <Label>{label}</Label>}
        <Container size={size} valuePosition={valuePosition}>
          <CurrencyInput
            ref={ref}
            className="decimal-input"
            defaultValue={defaultValue}
            allowNegativeValue={allowNegativeValue}
            allowDecimals={allowDecimals}
            decimalSeparator={config?.decimalSeparator || DefaultDecimalConfig.decimalSeparator}
            groupSeparator={config?.thousandSeparator || DefaultDecimalConfig.thousandSeparator}
            decimalsLimit={
              decimalsLimit || config?.amountPrecision || DefaultDecimalConfig.amountPrecision
            }
            //fixedDecimalLength={decimalsLimit || config?.amountPrecision || DefaultDecimalConfig.amountPrecision}
            inputtype={inputType}
            maxLength={max}
            onChange={onChange}
            value={value}
            disabled={disabled}
            onValueChange={(value, name, values) => handleOnValueChange(value, name, values)}
            name={name}
            onFocus={({ currentTarget }) => {
              if (selectOnFocus) {
                currentTarget.select()

                return
              }

              currentTarget.setSelectionRange(
                currentTarget.value.length,

                currentTarget.value.length
              )
            }}
            {...props}
          />

          {!disabled && hasClearButton && (
            <div className="clear" onClick={handleOnClear}>
              <IconCircleRemove size="m" color={disabled ? 'var(--text-default)' : 'black'} />
            </div>
          )}

          {sufix && (
            <>
              <div className="separator" />

              <div className={`sufix ${disabled && 'disabled'}`}>{sufix}</div>
            </>
          )}
        </Container>
      </div>
    )
  }
)

export default DecimalInput
