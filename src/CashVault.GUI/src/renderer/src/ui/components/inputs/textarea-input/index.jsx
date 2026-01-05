/* eslint-disable prettier/prettier */
/* eslint-disable react/prop-types */

import styled from '@emotion/styled'
import { forwardRef } from 'react'
import IconClear from '@ui/components/icons/IconClear'

const Container = styled.div`
  background-color: var(--bg-dark);
  border-radius: var(--border-radius-input);
  display: flex;
  gap: 1rem;
  align-items: center;
  padding: 0 1.25rem;
  position: relative;

  & .text-input {
    outline: none;
    border: none;
    background-color: var(--bg-dark);
    width: 100%;
    height: 100%;
    font-size: 3.5rem;
    font-weight: 500;
    line-height: 4.625rem;
    padding-top: 1rem;
    padding-bottom: 1rem;
    border-radius: var(--border-radius-input);

    ${(p) => {
      if (p.size === 'm') {
        return `
          font-weight: 500;
          font-size: 1rem;
          line-height: 1.075rem;
          padding-top: 1.25rem;
          padding-bottom: 1.25rem;  
        `
      }
    }}

    &:active {
      outline: none;
    }
  }

  .btn-clear {
    border: none;
    cursor: pointer;
    margin: 0;
    top: 1rem;
    right: 1rem;
    display: flex;
    align-items: center;
    justify-content: center;
    position: absolute;
    gap: 0.5rem;
    font-weight: 500;
    background-color: var(--bg-dark);
    border-radius: var(--border-radius-input);
    border: 1px solid var(--border-dark);
    color: var(--text-dark);
    & p {
      margin-left: 0.5rem;
    }
    & .icon {
      width: 2.5rem;
      height: 2.5rem;
    }
  }
`

const OuterContainer = styled.div`
  display: flex;
  flex-direction: column;
  gap: 0.5rem;

  & label {
    padding-left: 0.125rem;
    color: #6a6a6a;
    font-weight: 500;
    text-transform: uppercase;

    ${(p) => {
      if (p.size === 'm') {
        return `
          font-size: 0.75rem;
          line-height: 1rem;
        `
      }
    }}

    ${(p) => {
      if (p.size === 'l') {
        return `
          font-size: 1rem;
          line-height: 1.25rem;
        `
      }
    }}
  }
`

const Input = styled.textarea`
  outline: none;
  border: none;
  background-color: var(--bg-dark);
  width: 100%;
  height: 100%;

  &:active {
    outline: none;
  }

  ${(p) => {
    if (p.size === 'm') {
      return `
        font-weight: 500;
        font-size: 1.25rem;
      `
    }
  }}

  ${(p) => {
    if (p.size === 'l') {
      return `
        font-weight: 500;
        font-size: 1.5rem;
      `
    }
  }}
`

const ClearButton = ({ onClear }) => {
  return (
    <div className="btn-clear" onClick={onClear} role="button" tabIndex={0}>
      <p>Clear</p>
      <div className="icon">
        <IconClear size="m" />
      </div>
    </div>
  )
}

const TextareaInput = forwardRef(
  (
    {
      size = 'l',
      defaultValue,
      value,
      disabled = false,
      onChange = () => {},
      label,
      placeholder,
      rows = 4,
      outerContainerClassName = '',
      inputContainerClassName = '',
      className = '',
      hasClearButton = false,
      onClear = () => {},
      rest
    },
    ref
  ) => {
    return (
      <OuterContainer size={size} className={outerContainerClassName}>
        {label && <label>{label}</label>}

        <Container size={size} className={inputContainerClassName}>
          {hasClearButton && <ClearButton onClear={onClear} />}
          <Input
            className={className}
            size={size}
            value={value}
            defaultValue={defaultValue}
            onChange={onChange}
            disabled={disabled}
            placeholder={placeholder}
            rows={rows}
            ref={ref}
            {...rest}
          />
        </Container>
      </OuterContainer>
    )
  }
)

export default TextareaInput
