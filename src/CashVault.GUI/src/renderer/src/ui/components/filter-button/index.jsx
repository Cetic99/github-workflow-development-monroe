/* eslint-disable prettier/prettier */
/* eslint-disable react/prop-types */

import styled from '@emotion/styled'
import IconFilter from '@ui/components/icons/IconFilter'
import { hasAnyValue } from '@domain/global/services'
import { isEmpty } from 'lodash'

const ColorMap = {
  dark: {
    bg: 'var(--primary-medium)',
    bgActive: 'var(--primary-dark)',
    text: 'white',
    icon: 'white'
  },
  light: {
    bg: 'var(--text-medium)',
    bgActive: '#798B6E',
    text: 'white',
    icon: 'white'
  }
}

const Container = styled.div`
  position: relative;
  display: inline-block;
  border-radius: 0.625rem;
  border: none;
  min-height: 3.5rem;

  font-weight: 600;
  font-size: 1.5rem;
  line-height: 1.875rem;
  letter-spacing: -2%;
  color: ${(p) => ColorMap[p.color].text};
  background-color: ${(p) => (p.filterApplied ? 'var(--error-dark)' : ColorMap[p.color].bg)};

  & .circle-cutout {
    position: absolute;
    top: -0.625rem;
    right: -0.625rem;
    width: 1.5rem;
    height: 1.5rem;
    background-color: var(--bg-medium);
    border-radius: 50%;
    z-index: 1;
  }

  & .circle-dot {
    position: absolute;
    top: -0.25rem;
    right: -0.25rem;
    width: 0.625rem;
    height: 0.625rem;
    background-color: var(--error-dark);
    border-radius: 50%;
    z-index: 2;
  }
`

const InnerContainer = styled.button`
  width: fit-content;
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  transition: all 0.25s ease;
  position: relative;
  background-color: transparent;
  color: white;

  border-radius: 0.625rem;
  border: none;
  padding: 0 1.25rem;
  min-height: 3.5rem;

  font-weight: 600;
  font-size: 1.5rem;
  line-height: 1.875rem;
  letter-spacing: -2%;

  &:disabled {
    opacity: 0.5;
  }
`

const FilterButton = ({
  className = '',
  children,
  disabled = false,
  color = 'dark',
  onClick = () => {},
  filters = {}
}) => {
  const filterApplied = !isEmpty(filters) && hasAnyValue(filters)

  return (
    <Container
      filterApplied={filterApplied}
      className={className}
      onClick={onClick}
      disabled={disabled}
      color={color}
    >
      <InnerContainer>
        <IconFilter color={ColorMap[color].icon} size="m" />
        {children}
      </InnerContainer>
      {filterApplied && (
        <>
          <div className="circle-cutout"></div>
          <div className="circle-dot"></div>
        </>
      )}
    </Container>
  )
}

export default FilterButton
