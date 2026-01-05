/* eslint-disable prettier/prettier */
/* eslint-disable react/prop-types */

import styled from '@emotion/styled'

const Container = styled.div`
  .toggle {
    position: relative;
    display: inline-block;
    width: 4.375rem;
    height: 2.125rem;
  }

  .toggle input {
    display: none;
  }

  .slider {
    position: absolute;
    cursor: pointer;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background-color: var(--bg-gray-medium);
    transition: 0.4s;
    border-radius: 2.125rem;
  }

  .slider:before {
    position: absolute;
    content: '';
    height: 1.625rem;
    width: 1.625rem;
    left: 0.25rem;
    bottom: 0.25rem;
    background-color: var(--secondary-light);
    transition: 0.4s;
    border-radius: 50%;
  }

  input:checked + .slider {
    background-color: var(--secondary-dark);
  }

  input:checked + .slider:before {
    transform: translateX(2.125rem);
  }
`

const Toggle = ({ checked = false, onClick = () => {}, ...rest }) => {
  return (
    <Container {...rest}>
      <label className="toggle">
        <input
          type="checkbox"
          id="btnToggle"
          name="btnToggle"
          checked={checked}
          onChange={onClick}
        />
        <span className="slider" />
      </label>
    </Container>
  )
}

export default Toggle
