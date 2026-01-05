/* eslint-disable react/prop-types */
/* eslint-disable react/display-name */
/* eslint-disable prettier/prettier */
/* eslint-disable react/prop-types */

import styled from '@emotion/styled'
import { forwardRef } from 'react'

const Container = styled.div`
  background-color: var(--bg-dark);
  border-radius: var(--border-radius-input);
  display: flex;
  gap: 1rem;
  align-items: center;
  padding: 0 1.25rem;

  & .text-input {
    outline: none;
    border: none;
    background-color: var(--bg-dark);
    width: 100%;
    height: 100%;
    font-size: 3.5rem;
    font-weight: 700;
    line-height: 4.625rem;
    padding-top: 1rem;
    padding-bottom: 1rem;
    border-radius: var(--border-radius-input);

    ${(p) => {
      if (p.size === 'm') {
        return `
          font-weight: 600;
          font-size: 1.125rem;
          line-height: 1.375rem;
          padding-top: 1.475rem;
          padding-bottom: 1.475rem;  
        `
      }
    }}

    &:active {
      outline: none;
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
    font-weight: 600;
    text-transform: uppercase;

    ${(p) => {
      if (p.size === 'm') {
        return `
          font-size: 1rem;
          line-height: 1.25rem;
        `
      }
    }}

    ${(p) => {
      if (p.size === 'l') {
        return `
          font-size: 1.125rem;
          line-height: 1.375rem;
        `
      }
    }}
  }
`

const Input = styled.input`
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
        font-weight: 600;
        font-size: 1.5rem;
        line-height: 2.25rem;
        min-height: 4.625rem;
        max-height: 4.625rem;
      `
    }
  }}

  ${(p) => {
    if (p.size === 'l') {
      return `
        font-weight: 700;
        font-size: 2rem;
        line-height: 2.75rem;
        min-height: 6.25rem;
        max-height: 6.25rem;
      `
    }
  }}
`

const TextInput = forwardRef(
  (
    {
      size = 'l',
      defaultValue,
      value,
      disabled = false,
      isPassword = false,
      onChange = () => {},
      label,
      placeholder,
      selectOnFocus = true,
      className = '',
      ...rest
    },
    ref
  ) => {
    return (
      <OuterContainer size={size} className={className}>
        {label && <label>{label}</label>}

        <Container size={size}>
          <Input
            type={isPassword ? 'password' : 'text'}
            size={size}
            value={value}
            defaultValue={defaultValue}
            onChange={onChange}
            disabled={disabled}
            placeholder={placeholder}
            ref={ref}
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
            {...rest}
          />
        </Container>
      </OuterContainer>
    )
  }
)

export default TextInput
