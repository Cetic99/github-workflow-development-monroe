/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import Select from 'react-select'
import { useTranslation } from '@domain/administration/stores'

const Container = styled.div`
  display: flex;
  flex-direction: row-reverse;
  gap: 1rem;

  & .description {
    font-family: Poppins;
    font-weight: 500;
    font-size: 1.25rem;
    line-height: 1.25rem;
    letter-spacing: -2%;

    align-content: center;
  }

  & .light-txt {
    color: var(--secondary-dark);
  }

  & .dark-txt {
    color: var(--primary-dark);
  }

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
    width: fit-content;

    & .info {
      width: 4rem;
      height: 100%;
      text-align: center;
    }

    > div {
      width: 100%;
    }
  }
`

const customStyles = {
  control: (base, state) => ({
    ...base,
    backgroundColor: '#CCD4C7',
    border: 'none',
    boxShadow: 'none',
    borderRadius: '4px',
    padding: '0.125rem 0.5rem',
    cursor: 'pointer',
    fontFamily: 'Poppins',
    fontWeight: '600',
    fontSize: '1.25rem',
    lineHeight: '1.75rem',
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
      width: '20px',
      height: '20px'
    }
  }),
  menu: (base) => ({
    ...base,
    backgroundColor: '#ccd5c5',
    borderRadius: '4px',
    marginTop: '0.25rem',
    boxShadow: 'none',
    padding: 0,
    zIndex: 9999 // Ensure the dropdown is above other elements
  }),
  option: (base, state) => ({
    ...base,
    backgroundColor: '#CCD4C7',
    color: '#000',
    fontWeight: state.isSelected ? '600' : '400',
    cursor: 'pointer',
    borderRadius: '4px',
    fontFamily: 'Poppins',
    fontSize: '1.25rem',
    lineHeight: '1.75rem',
    padding: '0.375rem 1rem',
    ':active': {
      backgroundColor: '#ccd5c5'
    }
  })
}

const ItemsPerPage = ({
  disabled = false,
  className,
  onChange,
  items = [5, 10, 15, 20, 25, 50, 100],
  value = 10,
  totalCount,
  page
}) => {
  const { t } = useTranslation()

  const formattedOptions = (items || []).map((item) => ({
    value: item,
    label: t('rows per page') + ': ' + item
  }))

  const shownFrom = (page - 1) * value + 1
  const shownTo = (page - 1) * value + value

  return (
    <Container className={className}>
      <div className="description">
        <span className="dark-txt">{`${shownFrom} - ${totalCount < shownTo ? totalCount : shownTo}`}</span>
        <span className="light-txt">{` ${t('of')} ${totalCount} ${t('rows')}`}</span>
      </div>
      <div className="dropdown-input">
        <Select
          isDisabled={disabled}
          options={formattedOptions}
          value={formattedOptions.find((opt) => opt.value === value)}
          onChange={onChange}
          isClearable={false}
          styles={customStyles}
          isSearchable={false}
        />
      </div>
    </Container>
  )
}

export default ItemsPerPage
