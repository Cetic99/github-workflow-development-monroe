/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'

const Container = styled.div`
  color: var(--text-default);
  font-size: 1rem;
  line-height: 1.125rem;
  font-weight: 600;
  display: flex;
  align-items: center;

  &::before {
    flex: 1;
    content: '';
    padding: 0.5px;
    background-color: var(--bg-divider);
    margin: 0.75rem 0.75rem 0.75rem 0;
  }

  &::after {
    flex: 1;
    content: '';
    padding: 0.5px;
    background-color: var(--bg-divider);
    margin: 0.75rem 0 0.75rem 0.75rem;
  }
`

const TextDivider = ({ children }) => {
  return <Container>{children}</Container>
}

export default TextDivider
