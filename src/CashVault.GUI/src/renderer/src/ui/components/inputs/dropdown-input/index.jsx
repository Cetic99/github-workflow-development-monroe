/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import Select from 'react-select'
import IconInfoSquare from '@ui/components/icons/IconInfoSquare'
import { useRef } from 'react'
import Modal from '@ui/components/modal'

const Container = styled.div`
  & .label {
    color: var(--text-default);

    padding-left: 0.125rem;
    font-weight: 600;
    font-size: 1.125rem;
    line-height: 1.375rem;
    text-transform: uppercase;
    margin-bottom: 0.5rem;
  }

  & .dropdown-input {
    display: flex;
    align-items: center;
    background-color: var(--text-medium);
    border: none;
    box-shadow: none;
    box-sizing: border-box;
    border-radius: 0.875rem;

    & .info {
      width: 4rem;
      height: 100%;
      text-align: center;
    }

    width: 100%;
    > div {
      width: 100%;
    }
  }

  width: 100%;
`

const ModalContent = styled.div`
  & .modal-header {
    font-weight: 600;
    font-size: 1.75rem;
    line-height: 1.875rem;
    margin-bottom: 1rem;
  }
`

const customStyles = {
  control: (base, state) => ({
    ...base,
    backgroundColor: '#ccd5c5',
    border: 'none',
    boxShadow: 'none',
    borderRadius: '12px',
    padding: '0.8375rem 1rem',
    cursor: 'pointer',
    fontFamily: 'Poppins',
    fontWeight: '600',
    fontSize: '1.625rem',
    lineHeight: '2.20rem',
    opacity: state.isDisabled ? 0.6 : 1
  }),
  singleValue: (base) => ({
    ...base,
    fontWeight: '600',
    color: '#0f1f17' // dark greenish-black
  }),
  indicatorSeparator: () => ({ display: 'none' }),
  dropdownIndicator: (base, state) => ({
    ...base,
    color: 'var(--primary-dark)', // arrow color
    transform: state.selectProps.menuIsOpen ? 'rotate(180deg)' : null,
    transition: 'transform 0.2s ease',
    padding: '0.25rem',
    svg: {
      width: '36px',
      height: '36px'
    }
  }),
  menu: (base) => ({
    ...base,
    backgroundColor: '#ccd5c5',
    borderRadius: '10px',
    marginTop: '0.25rem',
    boxShadow: 'none',
    padding: 0,
    zIndex: 9999 // Ensure the dropdown is above other elements
  }),
  option: (base, state) => ({
    ...base,
    backgroundColor: '#ccd5c5',
    color: '#000',
    fontWeight: state.isSelected ? '600' : '400',
    cursor: 'pointer',
    borderRadius: '10px',
    fontFamily: 'Poppins',
    fontSize: '1.625rem',
    lineHeight: '2.25rem',
    padding: '1rem 1.5rem',
    ':active': {
      backgroundColor: '#ccd5c5'
    }
  })
}

const DropdownInput = ({
  name,
  label,
  placeholder,
  disabled = false,
  className,
  selectedOption,
  onChange,
  options = [],
  optionValueName = 'value',
  optionLabelName = 'name',
  value,
  info
}) => {
  const infoModalRef = useRef()

  const formattedOptions = (options || []).map((option) => ({
    ...option,
    value: option[optionValueName],
    label: optionLabelName ? option[optionLabelName] : option[optionValueName]
  }))

  const handleOpenInfoModal = () => {
    if (infoModalRef.current) {
      infoModalRef.current.showModal()
    }
  }

  return (
    <Container className={className}>
      <p className="label">{label}</p>
      <div className="dropdown-input">
        <Select
          isDisabled={disabled}
          isSearchable={false}
          name={name}
          options={formattedOptions}
          value={selectedOption || formattedOptions.find((opt) => opt.value === value)}
          onChange={onChange}
          placeholder={placeholder}
          isClearable={false}
          styles={customStyles}
        />
        {info && (
          <div className="info" onClick={() => handleOpenInfoModal()}>
            <IconInfoSquare color="white" />
          </div>
        )}
      </div>
      <Modal
        size="m"
        ref={infoModalRef}
        onClose={() => {
          infoModalRef?.current?.close()
        }}
      >
        <ModalContent>
          <div className="modal-header">Info</div>
          {info}
        </ModalContent>
      </Modal>
    </Container>
  )
}

export default DropdownInput
