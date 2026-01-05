/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'

import { WidgetSize } from '@domain/configuration/consts/user-widgets'
import Toggle from '@ui/components/toggle'
import { GetIcon } from '@src/app/element-loaders'
import { useMemo } from 'react'
import RadioItemInput from '@ui/components/inputs/radio-input'

const Container = styled.div`
  width: 100%;
  height: 10rem;
  display: flex;
  gap: 1rem;
  background-color: white;
  border-radius: 1rem;
  box-shadow: 0 0.125rem 0.25rem rgba(0, 0, 0, 0.25);

  .action-button {
    width: 5rem;
    height: 5rem;
    font-size: 1.25rem;
    border-radius: 50%;
  }

  .content {
    display: flex;
    flex-direction: column;
    gap: 1.5rem;
    width: 100%;
    padding: 1rem 1.25rem 0 0.25rem;
  }

  .title {
    font-size: 1.75rem;
    font-weight: 600;
    display: flex;
    gap: 1rem;
    align-items: center;
  }

  .info {
    display: flex;
    justify-content: space-between;
  }

  .drag-handle {
    align-content: center;
    justify-content: center;
    width: 2.5rem;
    background-color: var(--bg-gray-medium);
    align-items: center;
    font-size: 1.75rem;
    font-weight: 500;
    border-radius: 1rem 0 0 1rem;

    display: grid;
    grid-template-rows: repeat(4, auto);
    grid-template-columns: repeat(2, auto);
    column-gap: 0.5rem;
    row-gap: 0.125rem;
    font-size: 1.25rem;
    line-height: 0.5;
    cursor: grab;
    user-select: none;
    width: 2.5rem;
    height: 100%;
  }

  .toggle {
    display: flex;
    justify-content: center;
    align-items: center;
  }
`

const RadioGroup = styled.div`
  display: flex;
  gap: 3.5rem;

  .label {
    font-size: 1.25rem;
    font-weight: 500;
  }

  .option {
    align-self: center;
  }
`

const SIZE_OPTIONS = [
  { value: WidgetSize.S.code, label: WidgetSize.S.name },
  { value: WidgetSize.M.code, label: WidgetSize.M.name },
  { value: WidgetSize.L.code, label: WidgetSize.L.name }
]

const WidgetItem = ({ uuid, index, code, size, enabled, className, onPropChange, icon }) => {
  const memoizedIcon = useMemo(() => GetIcon(code), [code])

  return (
    <>
      <Container className={className}>
        <div className="drag-handle" data-swapy-handle>
          <span>&middot;</span> <span>&middot;</span> <span>&middot;</span> <span>&middot;</span>{' '}
          <span>&middot;</span> <span>&middot;</span> <span>&middot;</span> <span>&middot;</span>
        </div>
        <div className="content">
          <div className="title">
            {memoizedIcon}
            {code}
          </div>
          <div className="info">
            <RadioGroup>
              {SIZE_OPTIONS.map((option, i) => (
                <RadioItemInput
                  key={`size-option-${i}`}
                  value={size === option.value}
                  label={option.label}
                  className="option"
                  onChange={() => {
                    if (size === option.value) return

                    onPropChange(uuid, index, 'size', option.value)
                  }}
                />
              ))}
            </RadioGroup>
            <Toggle
              checked={enabled}
              className="toggle"
              onClick={() => onPropChange(uuid, index, 'enabled', !enabled)}
            />
          </div>
        </div>
      </Container>
    </>
  )
}

export default WidgetItem
