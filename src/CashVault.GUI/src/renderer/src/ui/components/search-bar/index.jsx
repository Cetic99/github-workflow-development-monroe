/* eslint-disable react/prop-types */
import styled from '@emotion/styled'
import IconSearch from '@ui/components/icons/IconSearch'
import { useState } from 'react'

const SearchContainer = styled.div`
  display: flex;
  align-items: center;
  position: relative;
  justify-content: center;
  width: 100%;
  background-color: white;
  padding: 1.625rem;
  border-radius: 1rem;
  box-shadow: 0px 24px 40px 0px rgba(198, 205, 196, 1);

  cursor: ${(p) => (p.interactable === false ? 'pointer' : 'text')};

  svg {
    position: absolute;
    left: 1.625rem;
  }

  .search-input {
    all: unset;
    height: 2.3rem;
    display: block;
    width: 65%;
    font-size: 2rem;
    line-height: 2rem;
    font-weight: 700;
    font-family: inherit;
    color: black;
  }

  .search-input::placeholder {
    color: var(--text-default);
    font-weight: 700;
    opacity: 1;
  }

  .search-input:disabled {
    pointer-events: none;
    cursor: default;
    opacity: 1;
  }
`

const SearchBar = ({ onClick = () => {}, interactable = false, onChange = () => {} }) => {
  const [search, setSearch] = useState('')

  const handleOnChange = (e) => {
    setSearch(e.target.value)
    onChange(e.target.value)
  }

  return (
    <SearchContainer onClick={onClick} interactable={interactable}>
      <IconSearch size="l" color="#97AA8B" />
      <input
        className="search-input"
        placeholder="Welcome, tap here to search..."
        disabled={!interactable}
        value={search}
        onChange={handleOnChange}
      />
    </SearchContainer>
  )
}

export default SearchBar
