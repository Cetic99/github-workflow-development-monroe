/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import { WidgetSize } from '@domain/configuration/consts/user-widgets'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  gap: 3rem;
  user-select: none;
  cursor: ${(props) => (props.disabled ? 'not-allowed' : 'pointer')};
  transition: all 0.25s ease;

  box-sizing: border-box;
  padding: 1.25rem;
  border-radius: var(--border-radius);

  ${(p) => {
    let size = WidgetSize.S

    if (p.size && Object.keys(WidgetSize).includes(p.size)) {
      size = WidgetSize[p.size]
    }

    if (size.code === WidgetSize.L.code) {
      return `flex: 0 0 ${size.width};
      max-width: ${size.width};
      `
    }

    return `flex: 0 0 calc(${size.width} - (${p.gap} / ${size.fraction}));
      max-width: calc(${size.width} - (${p.gap} / ${size.fraction}));`
  }}

  background-color: ${(props) =>
    !props.disabled ? 'var(--secondary-dark)' : 'var(--disabled-bg)'};

  &:active {
    background-color: var(--primary-dark);
  }

  &:active {
    background-color: ${(props) =>
      !props.disabled ? 'var(--primary-dark)' : 'var(--disabled-bg)'};
  }

  & .card-button-header {
    line-height: 0;
  }

  & .card-button-content {
    margin-top: auto;
    font-size: 2.125rem;
    line-height: 2.375rem;
    font-weight: 600;
    color: white;
  }
`

const Widget = ({ icon, text, onClick, disabled = false, size, gap = '0', ...rest }) => {
  const handleClick = (event) => {
    if (disabled) {
      event.stopPropagation()
      return
    }

    onClick(event)
  }

  return (
    <Container onClick={handleClick} disabled={disabled} size={size} gap={gap} {...rest}>
      <div className="card-button-header">{icon}</div>
      <div className="card-button-content">{text}</div>
    </Container>
  )
}

export default Widget
